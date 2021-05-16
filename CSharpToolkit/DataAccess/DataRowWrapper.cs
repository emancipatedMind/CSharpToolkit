namespace CSharpToolkit.DataAccess {
    using System;
    using System.Data;
    using System.Linq;

    /// <summary>
    /// A class that is a wrapper for the DataRow class. Offers strong typing access to the data row fields, and assignable aliases.
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    public class DataRowWrapper : PropertyStore {

        /// <summary>
        /// Instantiates the <see cref="DataRowWrapper"/> class.
        /// </summary>
        public DataRowWrapper() { }

        /// <summary>
        /// Instantiates the <see cref="DataRowWrapper"/> class.
        /// </summary>
        public DataRowWrapper(System.Type type) { }

        /// <summary>
        /// Instantiates the <see cref="DataRowWrapper"/> class.
        /// </summary>
        /// <param name="row">The data row that is wrapped.</param>
        public DataRowWrapper(DataRow row) {
            System.Diagnostics.Debug.Assert(row != null, $"{typeof(DataRow).AssemblyQualifiedName} passed into {nameof(DataRowWrapper)} cannot be null.");

            _propertyStore.AddRange(
                row.Table.Columns.Cast<DataColumn>()
                    .Select(col => new { Name = col.ColumnName, Value = row[col] })
                    .Select(
                        x => {
                            var p = new PropertyItem(x.Name) { Current = x.Value is DBNull ? null : x.Value, Original = x.Value is DBNull ? null : x.Value };
                            return p;
                        }
                    )
            );
        }

    }
}