namespace CSharpToolkit.ViewModels.Abstractions {
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using DataAccess;
    using DataAccess.Abstractions;
    using Utilities;
    using Extensions;
    using DataAccess.MetaData;
    using Utilities.Abstractions;

    public abstract class EditableBase<TModel> : ValidationBase, IModifyableChangeDescriptor, IIdProvider {

        #region Static Members
        static readonly ReadOnlyCollection<Tuple<string, FieldInfo>> _assignDictionary;
        Locker _loadingLocker = new Locker();

        static EditableBase() {
            Type genericType = typeof(TModel);
            Type baseClass = typeof(EditableBase<>).MakeGenericType(new[] { genericType });

            /*
            Item1 => Type of property being searched for.
            Item2 => Type of attribute searching for.
            Item3 => Whether null is allowed for property.
            Item4 => Field Info.
            */

            _assignDictionary =
                new ReadOnlyCollection<Tuple<string, FieldInfo>>(
                    new[] {
                        Tuple.Create(typeof(int), typeof(IdAttribute), false, baseClass.GetField(nameof(_idProperty), BindingFlags.NonPublic | BindingFlags.Instance)),
                    }
                    .Select(tuple => {
                        string propertyName =
                            genericType
                                .GetProperties()
                                .Where(prop => prop.PropertyType == tuple.Item1)
                                .FirstOrDefault(prop => Attribute.IsDefined(prop, tuple.Item2))?
                                .Name;

                        System.Diagnostics.Debug.Assert(tuple.Item3 || string.IsNullOrEmpty(propertyName) == false, $"{genericType.FullName} does not have a property with the {tuple.Item2.FullName} attribute.");

                        return Tuple.Create(propertyName, tuple.Item4);
                    })
                    .ToList()
                );
        }

        readonly List<PropertyInfo> ModelProperties = new List<PropertyInfo>();
        readonly List<string> PropertyNames = new List<string>();

        private static void ValidateType(object obj) {
            Type classType = obj.GetType();
            Type genericType = typeof(TModel);
            Type baseClass = typeof(EditableBase<>).MakeGenericType(new[] { genericType });

            if (genericType.IsInterface) {
                System.Diagnostics.Debug.Assert(genericType.IsAssignableFrom(classType), $"{classType.AssemblyQualifiedName} must implement its generic parameter type {genericType.AssemblyQualifiedName}.");
            }
            else {
                System.Diagnostics.Debug.Assert(classType.IsSubclassOf(genericType), $"{classType.AssemblyQualifiedName} must be a subclass of the generic parameter type {genericType.AssemblyQualifiedName}.");
            }

            _assignDictionary.Where(tuple => tuple.Item1.IsMeaningful()).ForEach(tuple => tuple.Item2.SetValue(obj, classType.GetProperty(tuple.Item1)));
        }
        #endregion

#pragma warning disable CS0649
        PropertyInfo _idProperty;
#pragma warning restore CS0649

        IModifyableChangeDescriptor _modifyableModel;

        public EditableBase() {
            ValidateType(this);
            ModelProperties.AddRange(
                from thisProp in GetType().GetProperties()
                join mProp in typeof(TModel).GetProperties() on thisProp.Name equals mProp.Name
                select thisProp
            );

            PropertyNames.AddRange(ModelProperties.Select(mProp => mProp.Name));
        }

        protected IModifyableChangeDescriptor ModifyableModel {
            get { return _modifyableModel ?? NullModifyableChangeDescriptor.Instance; }
            set { Perform.ReplaceIfDifferent(ref _modifyableModel, value); }
        }

        public int Id {
            get { return (int)(_idProperty.GetValue(this) ?? 0); }
            set { _idProperty.SetValue(this, value); }
        }

        public bool Modified => ModifyableModel.Modified;

        public void Load(TModel model) {
            if (_loadingLocker.IsLocked())
                return;
            _loadingLocker.LockForDuration(() =>
                PropertyNames.ForEach(
                    name => Perform.PropertyAssignmentThroughReflection(new KeyValuePair<object, string>(model, name), new KeyValuePair<object, string>(this, name))
                )
            );
        }

        public void Clear() {
            ModelProperties.ForEach(prop => prop.SetValue(this, null));
        }

        public void Reset() {
            if (ModifyableModel is NullModifyableChangeDescriptor)
                return;

            PropertyNames.Select(name => {
                bool propReset = ModifyableModel.Reset(name);
                return new { PropReset = true, Name = name };
            })
            .Where(x => x.PropReset)
            .ForEach(x => OnPropertyChanged(x.Name));
        }

        public bool Reset(string name) {
            bool propReset = ModifyableModel.Reset(name);
            if (propReset)
                OnPropertyChanged(name);
            return propReset;
        }

        public void Save() => ModifyableModel.Save();

        public List<PropertyModification> GetChangedProperties() =>
            ModifyableModel.GetChangedProperties();

    }
}