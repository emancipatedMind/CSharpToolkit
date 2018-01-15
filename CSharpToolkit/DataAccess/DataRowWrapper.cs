namespace CSharpToolkit.DataAccess {
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    /// <summary>
    /// A base class that is a wrapper for the DataRow class. Offers strong typing access to the data row fields, and assignable aliases.
    /// </summary>
    public abstract class DataRowWrapper {

        DataRow _row;
        readonly List<KeyValuePair<string, string>> _aliases = new List<KeyValuePair<string, string>>();

        /// <summary>
        /// Instantiates the DataRow class.
        /// </summary>
        /// <param name="row">The data row that is wrapped.</param>
        public DataRowWrapper(DataRow row) {
            _row = row;
        }

        /// <summary>
        /// Access to the fields of wrapped DataRow.
        /// </summary>
        /// <typeparam name="T">Field type.</typeparam>
        /// <param name="name">Field name.</param>
        /// <returns>Data found. If no data found, returns DBNull.Value if nullable, or default if System.ValueType.</returns>
        protected T GetDataFromRow<T>(string name) {

            string field = _aliases.FirstOrDefault(alias => alias.Key == name).Value;

            if (string.IsNullOrEmpty(field))
                field = name;

            if (_row.Table.Columns.Contains(field) == false)
                return default(T);

            try { return _row.Field<T>(field); }
            catch(Exception ex) when (ex is ArgumentException || ex is IndexOutOfRangeException || ex is InvalidCastException) {
                return default(T);
            }

        }

        /// <summary>
        /// Add alias that can be used to access data.
        /// </summary>
        /// <param name="aliases">A collection of aliases to add.</param>
        public void AddAlias(IEnumerable<KeyValuePair<string, string>> aliases) =>
            _aliases.AddRange(aliases);

        /// <summary>
        /// Add alias that can be used to access data.
        /// </summary>
        /// <param name="alias">Alias to add.</param>
        public void AddAlias(KeyValuePair<string, string> alias) =>
            _aliases.Add(alias);

        /// <summary>
        /// Add alias that can be used to access data.
        /// </summary>
        /// <param name="column">Column name alias will reference.</param>
        /// <param name="alias">Alias to use.</param>
        public void AddAlias(string column, string alias) =>
            _aliases.Add(new KeyValuePair<string, string>(column, alias));

        /// <summary>
        /// Add alias that can be used to access data.
        /// </summary>
        /// <param name="alias">Alias to add.</param>
        public void AddAlias(Utilities.Abstractions.IAlias alias) =>
            _aliases.Add(alias.Data);

        /// <summary>
        /// Add alias that can be used to access data.
        /// </summary>
        /// <param name="aliases">Alias collection to add.</param>
        public void AddAlias(IEnumerable<Utilities.Abstractions.IAlias> aliases) =>
            _aliases.AddRange(aliases.Select(a => a.Data));

    }
}