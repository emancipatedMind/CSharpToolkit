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
    public class DataRowWrapper : IModifyableChangeDescriptor {

        DataRow _row;
        readonly Utilities.Aliaser _aliaser = new Utilities.Aliaser();

        /// <summary>
        /// Instantiates the DataRowWrapper class. One of the other constructors where the Row is set up beforehand is recommended.
        /// </summary>
        public DataRowWrapper() {
            var table = new DataTable();
            _row = table.NewRow();
            table.Rows.Add(_row);
            _row.BeginEdit();
        }

        /// <summary>
        /// Instantiates the DataRowWrapper class.
        /// </summary>
        /// <param name="row">The data row that is wrapped.</param>
        public DataRowWrapper(DataRow row) {
            System.Diagnostics.Debug.Assert(row != null, $"{typeof(DataRow).AssemblyQualifiedName} passed into {nameof(DataRowWrapper)} cannot be null.");

            var table = new DataTable();
            foreach (DataColumn column in row.Table.Columns) {
                table.Columns.Add(column.ColumnName, column.DataType);
            }
            _row = table.NewRow();
            table.Rows.Add(_row);
            _row.ItemArray = (object[])row.ItemArray.Clone();
            _row.AcceptChanges();
            _row.BeginEdit();
        }

        /// <summary>
        /// Instantiates the DataRowWrapper class. This overload allows a schema to be provided to the row wrapped underneath.
        /// </summary>
        /// <param name="schema">A collection of key value pairs used to generate the schema.</param>
        public DataRowWrapper(IEnumerable<Tuple<string, Type>> schema) {
            System.Diagnostics.Debug.Assert(schema != null, $"{typeof(Tuple<string, Type>).AssemblyQualifiedName} collection passed into {nameof(DataRowWrapper)} cannot be null.");
            SetUpRowUsingNameAndType(schema);
        }

        /// <summary>
        /// Instantiates the DataRowWrapper class. This overload allows a schema to be provided to the row wrapped underneath.
        /// </summary>
        /// <param name="schema">A collection of key value pairs used to generate the schema.</param>
        public DataRowWrapper(IEnumerable<KeyValuePair<string, Type>> schema) {
            System.Diagnostics.Debug.Assert(schema != null, $"{typeof(KeyValuePair<string, Type>).AssemblyQualifiedName} collection passed into {nameof(DataRowWrapper)} cannot be null.");
            SetUpRowUsingNameAndType(schema.Select(kvp => Tuple.Create(kvp.Key, kvp.Value)));
        }

        /// <summary>
        /// Instantiates the DataRowWrapper class. This overload will create a schema using property names, and types of the supplied type.
        /// </summary>
        /// <param name="properties">A collection of PropertyInfo classes used to generate the schema.</param>
        public DataRowWrapper(IEnumerable<PropertyInfo> properties) {
            System.Diagnostics.Debug.Assert(properties != null, $"{typeof(PropertyInfo).AssemblyQualifiedName} collection passed into {nameof(DataRowWrapper)} cannot be null.");
            SetUpRowUsingNameAndType(properties.Select(p => Tuple.Create(p.Name, p.PropertyType)));
        }

        /// <summary>
        /// Instantiates the DataRowWrapper class. This overload will create a schema using property names, and types of the supplied type.
        /// </summary>
        /// <param name="type">The type used to generate the schema.</param>
        public DataRowWrapper(Type type) {
            System.Diagnostics.Debug.Assert(type != null, $"{typeof(System.Type).AssemblyQualifiedName} passed into {nameof(DataRowWrapper)} cannot be null.");
            SetUpRowUsingNameAndType(
                type
                    .GetProperties()
                    .Select(p => Tuple.Create(p.Name, p.PropertyType))
                );
        }

        private void SetUpRowUsingNameAndType(IEnumerable<Tuple<string, Type>> schema) {
            var table = new DataTable();
            schema.ForEach(s => table.Columns.Add(s.Item1, Nullable.GetUnderlyingType(s.Item2) ?? s.Item2));
            _row = table.NewRow();
            table.Rows.Add(_row);
            _row.AcceptChanges();
            _row.BeginEdit();
        }

        /// <summary>
        /// Access to the original data of wrapped DataRow.
        /// </summary>
        /// <typeparam name="T">Field type.</typeparam>
        /// <param name="name">Field name.</param>
        /// <returns>Data found. If no data found, returns DBNull.Value if nullable, or default if System.ValueType.</returns>
        protected T GetOriginalValue<T>(string name) =>
            _row.HasVersion(DataRowVersion.Original) ?
                GetValue<T>(name, DataRowVersion.Original) :
                GetValue<T>(name, DataRowVersion.Current);

        /// <summary>
        /// Access to the current data of wrapped DataRow.
        /// </summary>
        /// <typeparam name="T">Field type.</typeparam>
        /// <param name="name">Field name.</param>
        /// <returns>Data found. If no data found, returns DBNull.Value if nullable, or default if System.ValueType.</returns>
        protected T GetCurrentValue<T>(string name) =>
            _row.HasVersion(DataRowVersion.Proposed) ?
                GetValue<T>(name, DataRowVersion.Proposed) :
                GetValue<T>(name, DataRowVersion.Current);

        /// <summary>
        /// Marks data so that a reset can be used to restore.
        /// </summary>
        void IModifyable.Save() {
            _row.AcceptChanges();
            _row.BeginEdit();
        }

        /// <summary>
        /// Reverts data to last saved state.
        /// </summary>
        void IModifyable.Reset() {
            _row.RejectChanges();
            _row.BeginEdit();
        }

        /// <summary>
        /// Whether any changes exist since last Save/Reset.
        /// </summary>
        bool IModifyable.Modified =>
            _row.Table.Columns.Cast<DataColumn>()
                .Any(column => HasFieldChanged(column.ColumnName));

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
        public void Reset(string name) {
            string field = _aliaser.LookUpAlias(name);

            if (_row.Table.Columns.Contains(field) == false)
                return;

            _row[field] = _row[field, GetVersion(DataRowVersion.Original)];
        }

        /// <summary>
        /// Used to set data of the wrapped data row.
        /// </summary>
        /// <typeparam name="T">Field type.</typeparam>
        /// <param name="name">Field name.</param>
        /// <param name="data">Data that is to be set.</param>
        protected void SetValue<T>(string name, T data) {
            string field = _aliaser.LookUpAlias(name);

            if (_row.Table.Columns.Contains(field) == false) {
                var table = _row.Table.Clone();
                table.Columns.Add(field, Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T));
                DataRow row = table.NewRow();
                foreach (DataColumn column in _row.Table.Columns) {
                    row[column.ColumnName] = _row[column, GetVersion(DataRowVersion.Original)];
                }
                table.Rows.Add(row);
                row.AcceptChanges();
                row.BeginEdit();
                foreach (DataColumn column in _row.Table.Columns) {
                    row[column.ColumnName] = _row[column, GetVersion(DataRowVersion.Proposed)];
                }
                _row = row;
            }

            _row.SetField(name, data);
        }

        /// <summary>
        /// Used to determine if a specified field has changed. If requested field is not contained in wrapped datarow, returns false.
        /// </summary>
        /// <param name="name">Field name.</param>
        /// <returns>Whether field name has changed or not.</returns>
        protected bool HasFieldChanged(string name) {
            string field = _aliaser.LookUpAlias(name);

            if (_row.Table.Columns.Contains(field) == false)
                return false;

            object original = _row[name, GetVersion(DataRowVersion.Original)];
            object proposed = _row[name, GetVersion(DataRowVersion.Proposed)];

            return proposed.Equals(original) == false;
        }

        private T GetValue<T>(string name, DataRowVersion version) {
            if (_row.HasVersion(version) == false)
                return default(T);

            string field = _aliaser.LookUpAlias(name);

            if (_row.Table.Columns.Contains(field) == false)
                return default(T);

            Type type = typeof(T);
            object value = _row[field, version];

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
                foreach (DataColumn column in _row.Table.Columns) {
                    if (HasFieldChanged(column.ColumnName)) {

                        string name = _aliaser.LookUpName(column.ColumnName);

                        Type type = column.DataType;

                        object newValue = GetValueForChangedProperty(name, DataRowVersion.Proposed, column);
                        object oldValue = GetValueForChangedProperty(name, DataRowVersion.Original, column);

                        list.Add(new PropertyModification(name, type, oldValue, newValue));
                    }
                }
            });

        object GetValueForChangedProperty(string name, DataRowVersion version, DataColumn type) {
            object value = _row[name, GetVersion(version)];
            return value is DBNull ?
                type.AllowDBNull ? null : Activator.CreateInstance(type.DataType) :
                value;
        }

        DataRowVersion GetVersion(DataRowVersion version) =>
            _row.HasVersion(version) ? version : DataRowVersion.Current;
    }
}