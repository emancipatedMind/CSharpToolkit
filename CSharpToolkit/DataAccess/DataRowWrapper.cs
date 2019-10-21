namespace CSharpToolkit.DataAccess {
    using Abstractions;
    using Extensions;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// A class that is a wrapper for the DataRow class. Offers strong typing access to the data row fields, and assignable aliases.
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    public class DataRowWrapper : IModifyableChangeDescriptor, IDisposable {
        bool _disposed;
        readonly Utilities.Aliaser _aliaser = new Utilities.Aliaser();
        List<PropertyStore> _propertyStore = new List<PropertyStore>();

        /// <summary>
        /// Instantiates the DataRowWrapper class.
        /// </summary>
        public DataRowWrapper() { }

        /// <summary>
        /// Instantiates the DataRowWrapper class.
        /// </summary>
        /// <param name="row">The data row that is wrapped.</param>
        public DataRowWrapper(DataRow row) {
            System.Diagnostics.Debug.Assert(row != null, $"{typeof(DataRow).AssemblyQualifiedName} passed into {nameof(DataRowWrapper)} cannot be null.");
            _propertyStore.AddRange(row.Table.Columns.Cast<DataColumn>().Select(col => new { Name = col.ColumnName, Value = row[col] }).Select(x => new PropertyStore(x.Name) { Current = x.Value, Original = x.Value }));
        }

        /// <summary>
        /// Instantiates the DataRowWrapper class. This overload will create a schema using property names, and types of the supplied type.
        /// </summary>
        /// <param name="type">The type used to generate the schema.</param>
        public DataRowWrapper(Type type) { }

        /// <summary>
        /// Access to the original data of wrapped DataRow.
        /// </summary>
        /// <typeparam name="T">Field type.</typeparam>
        /// <param name="name">Field name.</param>
        /// <returns>Data found. If no data found, returns DBNull.Value if nullable, or default if System.ValueType.</returns>
        protected T GetOriginalValue<T>(string name) =>
            GetValue<T>(name, RetrieveValue.Original);

        /// <summary>
        /// Access to the current data of wrapped DataRow.
        /// </summary>
        /// <typeparam name="T">Field type.</typeparam>
        /// <param name="name">Field name.</param>
        /// <returns>Data found. If no data found, returns DBNull.Value if nullable, or default if System.ValueType.</returns>
        protected T GetCurrentValue<T>(string name) =>
            GetValue<T>(name, RetrieveValue.Current);

        /// <summary>
        /// Marks data so that a reset can be used to restore.
        /// </summary>
        void IModifyable.Save() {
            _propertyStore.ForEach(prop => prop.Original = prop.Current);
        }

        /// <summary>
        /// Reverts data to last saved state.
        /// </summary>
        void IModifyable.Reset() {
            _propertyStore.ForEach(prop => prop.Current = prop.Original);
        }

        /// <summary>
        /// Whether any changes exist since last Save/Reset.
        /// </summary>
        bool IModifyable.Modified =>
            _propertyStore.Any(prop => prop.HasPropertyChanged());

        /// <summary>
        /// Add alias that can be used to access data.
        /// </summary>
        /// <param name="aliases">A collection of aliases to add.</param>
        public void AddAlias(IEnumerable<Tuple<string, string>> aliases) =>
            _aliaser.AddAlias(aliases);

        /// <summary>
        /// Add alias that can be used to access data.
        /// </summary>
        /// <param name="aliases">A collection of aliases to add.</param>
        public void AddAlias(IEnumerable<KeyValuePair<string, string>> aliases) =>
            _aliaser.AddAlias(aliases.Select(a => Tuple.Create(a.Key, a.Value)));

        /// <summary>
        /// Add alias that can be used to access data.
        /// </summary>
        /// <param name="alias">Alias to add.</param>
        public void AddAlias(KeyValuePair<string, string> alias) =>
            _aliaser.AddAlias(Tuple.Create(alias.Key, alias.Value));

        /// <summary>
        /// Add alias that can be used to access data.
        /// </summary>
        /// <param name="column">Column name alias will reference.</param>
        /// <param name="alias">Alias to use.</param>
        public void AddAlias(string column, string alias) =>
            _aliaser.AddAlias(Tuple.Create(column, alias));

        /// <summary>
        /// Add alias that can be used to access data.
        /// </summary>
        /// <param name="alias">Alias to add.</param>
        public void AddAlias(Utilities.Abstractions.IAlias alias) =>
            _aliaser.AddAlias(alias.Data);

        /// <summary>
        /// Add alias that can be used to access data.
        /// </summary>
        /// <param name="aliases">Alias collection to add.</param>
        public void AddAlias(IEnumerable<Utilities.Abstractions.IAlias> aliases) =>
            _aliaser.AddAlias(aliases.Select(a => a.Data));

        /// <summary>
        /// Used to reset only one field of the instance.
        /// </summary>
        /// <param name="name">The property to reset.</param>
        public bool Reset(string name) {
            string field = _aliaser.LookUpAlias(name);

            PropertyStore propStore = _propertyStore.FirstOrDefault(prop => prop.PropertyName == field);

            if (propStore != null) {
                bool propChanged = propStore.HasPropertyChanged();
                if (propChanged)
                    propStore.Current = propStore.Original;
                return propChanged;
            }

            return false;
        }

        /// <summary>
        /// Used to set data of the wrapped data row.
        /// </summary>
        /// <typeparam name="T">Field type.</typeparam>
        /// <param name="name">Field name.</param>
        /// <param name="data">Data that is to be set.</param>
        protected void SetValue<T>(string name, T data) {
            string field = _aliaser.LookUpAlias(name);

            PropertyStore propStore = _propertyStore.FirstOrDefault(prop => prop.PropertyName == field);
            if (propStore == null) {
                propStore = new PropertyStore(field);
                _propertyStore.Add(propStore);
            }
            propStore.Current = data;
            if (propStore.PropertyType == null)
                propStore.PropertyType = typeof(T);
        }

        private T GetValue<T>(string name, RetrieveValue retrieveValue) {
            string field = _aliaser.LookUpAlias(name);

            PropertyStore propStore = _propertyStore.FirstOrDefault(prop => prop.PropertyName == field);

            if (propStore == null)
                return default(T);

            Type type = typeof(T);
            object value = retrieveValue == RetrieveValue.Current ? propStore.Current : propStore.Original;

            bool notNullable = (
                type == typeof(string)
                || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            ) == false;

            if (value is DBNull && notNullable)
                return default(T);

            if (value.GetType() != (Nullable.GetUnderlyingType(type) ?? type))
                return default(T);

            return (T)value;
        }

        /// <summary>
        /// Gets properties that changed since save was last called.
        /// </summary>
        /// <returns>A list of property names that changed since last call.</returns>
        List<PropertyModification> IChangeDescriptor.GetChangedProperties() =>
            Utilities.Get.List<PropertyModification>(list => {
                MethodInfo method = null;
                foreach (PropertyStore store in _propertyStore) {
                    if (store.HasPropertyChanged()) {
                        string name = _aliaser.LookUpName(store.PropertyName);

                        var genericMethod =
                            (method ?? (method = typeof(DataRowWrapper).GetMethod(nameof(GetValue), BindingFlags.Instance | BindingFlags.NonPublic)))
                            .MakeGenericMethod(store.PropertyType);
                        object original = genericMethod.Invoke(this, new object[] { name, RetrieveValue.Original });
                        object current = genericMethod.Invoke(this, new object[] { name, RetrieveValue.Current });

                        list.Add(new PropertyModification(name, store.PropertyType, original, current));
                    }
                }
            });

        protected void Dispose(bool disposing) {
            if (!_disposed) {
                _aliaser.Dispose();
                _propertyStore.ForEach(prop => prop.Original = prop.Current = prop.PropertyType = null);
                _propertyStore.Clear();
            }
            _disposed = true;
        }

        public void Dispose() => Dispose(true);

        enum RetrieveValue {
            Current,
            Original
        }

        class PropertyStore {

            object _original;
            object _current;

            public PropertyStore(string name) {
                PropertyName = name;
            }

            public string PropertyName { get; }
            public Type PropertyType { get; set; }
            public object Current { get { return _current ?? DBNull.Value; } set { _current = value; } }
            public object Original { get { return _original ?? DBNull.Value; } set { _original = value; } }

            public bool HasPropertyChanged() => Current.Equals(Original) == false;
        }
    }
}