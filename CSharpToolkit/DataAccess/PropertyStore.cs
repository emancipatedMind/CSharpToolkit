namespace CSharpToolkit.DataAccess {
    using Abstractions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    public class PropertyStore : IModifyableChangeDescriptor, IDisposable, INotifyPropertyChanged {

        bool _disposed;
        protected readonly Utilities.Aliaser _aliaser = new Utilities.Aliaser();
        protected List<PropertyItem> _propertyStore = new List<PropertyItem>();

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Access to the original data of wrapped DataRow.
        /// </summary>
        /// <typeparam name="T">Field type.</typeparam>
        /// <param name="name">Field name.</param>
        /// <returns>Data found. If no data found, returns DBNull.Value if nullable, or default if System.ValueType.</returns>
        public T GetOriginalValue<T>(string name) =>
            GetValue<T>(name, RetrieveType.Original);

        /// <summary>
        /// Access to the current data of wrapped DataRow.
        /// </summary>
        /// <typeparam name="T">Field type.</typeparam>
        /// <param name="name">Field name.</param>
        /// <returns>Data found. If no data found, returns DBNull.Value if nullable, or default if System.ValueType.</returns>
        public T GetCurrentValue<T>(string name) =>
            GetValue<T>(name, RetrieveType.Current);

        /// <summary>
        /// Marks data so that a reset can be used to restore.
        /// </summary>
        public void Save() =>
            _propertyStore.ForEach(prop => prop.Save());

        /// <summary>
        /// Whether any changes exist since last Save/Reset.
        /// </summary>
        public bool Modified =>
            _propertyStore.Any(prop => prop.HasPropertyChanged);

        #region Adding Aliases
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
        #endregion

        /// <summary>
        /// Reverts data to last saved state.
        /// </summary>
        public void Reset() =>
            _propertyStore.ForEach(prop => prop.Reset());

        /// <summary>
        /// Used to reset only one field of the instance.
        /// </summary>
        /// <param name="name">The property to reset.</param>
        /// <returns>Whether the property was eligible to be reset.</returns>
        public bool Reset(string name) {
            string field = _aliaser.LookUpAlias(name);

            PropertyItem propStore = _propertyStore.FirstOrDefault(prop => prop.PropertyName == field);

            if (propStore != null) {
                bool propChanged = propStore.HasPropertyChanged;
                if (propChanged)
                    propStore.Current = propStore.Original;
                return propChanged;
            }

            return false;
        }

        public void Clear() => _propertyStore.ForEach(prop => prop.Current = null);

        /// <summary>
        /// Gets properties that changed since save was last called.
        /// </summary>
        /// <returns>A list of property names that changed since last call.</returns>
        public List<PropertyModification> GetChangedProperties() =>
            Utilities.Get.List<PropertyModification>(list => {
                MethodInfo method = null;
                foreach (PropertyItem store in _propertyStore.Where(prop => prop.HasPropertyChanged)) {
                    string name = _aliaser.LookUpName(store.PropertyName);

                    var genericMethod =
                        (method ?? (method = typeof(PropertyStore).GetMethod(nameof(GetValue), BindingFlags.Instance | BindingFlags.NonPublic)))
                            .MakeGenericMethod(store.PropertyType);
                    object original = genericMethod.Invoke(this, new object[] { name, RetrieveType.Original });
                    object current = genericMethod.Invoke(this, new object[] { name, RetrieveType.Current });

                    list.Add(new PropertyModification(name, store.PropertyType, original, current));
                }
            });

        public void Dispose() => Dispose(true);

        /// <summary>
        /// Used to set data of the wrapped data row.
        /// </summary>
        /// <typeparam name="T">Field type.</typeparam>
        /// <param name="name">Field name.</param>
        /// <param name="data">Data that is to be set.</param>
        public void SetValue<T>(string name, T data) {
            string field = _aliaser.LookUpAlias(name);

            PropertyItem propStore = _propertyStore.FirstOrDefault(prop => prop.PropertyName == field);
            if (propStore == null) {
                propStore = new PropertyItem(field);
                propStore.PropertyChanged += P_PropertyChanged;
                _propertyStore.Add(propStore);
            }
            if (propStore.PropertyType == null)
                propStore.PropertyType = typeof(T);

            propStore.Current = data;
        }

        public PropertyStore Clone() {
            var store = new PropertyStore();
            store._propertyStore.AddRange(this._propertyStore.Select(p => p.Clone()));
            return store;
        }

        private T GetValue<T>(string name, RetrieveType retrieveValue) {
            string field = _aliaser.LookUpAlias(name);

            PropertyItem propStore = _propertyStore.FirstOrDefault(prop => prop.PropertyName == field);

            if (propStore == null)
                return default(T);

            Type type = typeof(T);
            object value = retrieveValue == RetrieveType.Current ? propStore.Current : propStore.Original;

            return value == null || (value.GetType() != (Nullable.GetUnderlyingType(type) ?? type)) ?
                default(T) :
                (T)value;
        }

        private void P_PropertyChanged(object sender, PropertyChangedEventArgs e) =>
            PropertyChanged?.Invoke(this, e);

        protected void Dispose(bool disposing) {
            if (!_disposed) {
                PropertyChanged = null;
                _aliaser.Dispose();
                _propertyStore.ForEach(prop => { prop.PropertyChanged -= P_PropertyChanged; prop.Original = prop.Current = prop.PropertyType = null;});
                _propertyStore.Clear();
            }
            _disposed = true;
        }

    }
}
