namespace CSharpToolkit.DataAccess.Abstractions {
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using MetaData;
    using Extensions;
    using Utilities.Abstractions;
    using DataAccess;
    using System.Data;

    public class ModelBase<TModel> : DataRowWrapper, ICreatable, IIdProvider {

        #region Static Members
        static readonly ReadOnlyCollection<Tuple<string, FieldInfo>> _assignDictionary;

        static ModelBase() {
            Type genericType = typeof(TModel);
            Type baseClass = typeof(ModelBase<>).MakeGenericType(new[] { genericType });

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
                        Tuple.Create(typeof(string), typeof(CreatorAttribute), false, baseClass.GetField(nameof(_creatorProperty), BindingFlags.NonPublic | BindingFlags.Instance)),
                        Tuple.Create(typeof(string), typeof(UpdatorAttribute), true, baseClass.GetField(nameof(_updatorProperty), BindingFlags.NonPublic | BindingFlags.Instance)),
                        Tuple.Create(typeof(DateTime?), typeof(DateCreatedAttribute), false, baseClass.GetField(nameof(_dateCreatedProperty), BindingFlags.NonPublic | BindingFlags.Instance)),
                        Tuple.Create(typeof(DateTime?), typeof(DateUpdatedAttribute), true, baseClass.GetField(nameof(_dateUpdatedProperty), BindingFlags.NonPublic | BindingFlags.Instance)),
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

        private static void ValidateType(object obj) {
            Type classType = obj.GetType();
            Type genericType = typeof(TModel);
            Type baseClass = typeof(ModelBase<>).MakeGenericType(new[] { genericType });

            if (genericType.IsInterface) {
                System.Diagnostics.Debug.Assert(genericType.IsAssignableFrom(classType), $"{classType.AssemblyQualifiedName} must implement its generic parameter type {genericType.AssemblyQualifiedName}.");
            }
            else {
                System.Diagnostics.Debug.Assert(classType.IsSubclassOf(genericType), $"{classType.AssemblyQualifiedName} must be a subclass of the generic parameter type {genericType.AssemblyQualifiedName}.");
            }

            _assignDictionary.Where(tuple => tuple.Item1.IsMeaningful()).ForEach(tuple => tuple.Item2.SetValue(obj, classType.GetProperty(tuple.Item1)));
        }
        #endregion Static Members

#pragma warning disable CS0649
        PropertyInfo _idProperty;
        PropertyInfo _creatorProperty;
        PropertyInfo _dateCreatedProperty;
        PropertyInfo _updatorProperty;
        PropertyInfo _dateUpdatedProperty;
#pragma warning restore CS0649

        public ModelBase() : this(typeof(TModel)) { }

        public ModelBase(Type type) : base(type) {
            ValidateType(this);
        }

        public ModelBase(DataRow row) : base(row) {
            ValidateType(this);
        }

        public int Id {
            get { return (int)(_idProperty.GetValue(this) ?? 0); }
            set { _idProperty.SetValue(this, value); }
        }

        public string Creator {
            get { return (string)(_creatorProperty.GetValue(this) ?? ""); }
            set { _creatorProperty.SetValue(this, value); }
        }

        public string Updator {
            get { return (string)(_updatorProperty?.GetValue(this) ?? ""); }
            set { _updatorProperty?.SetValue(this, value); }
        }

        public DateTime? DateCreated {
            get { return (DateTime?)(_dateCreatedProperty.GetValue(this) ?? null); }
            set { _dateCreatedProperty.SetValue(this, value); }
        }

        public DateTime? DateUpdated {
            get { return (DateTime?)(_dateUpdatedProperty?.GetValue(this) ?? null); }
            set { _dateUpdatedProperty?.SetValue(this, value); }
        }

    }

}
