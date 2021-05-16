namespace CSharpToolkit.DataAccess.Request {
    using System;
    using System.Collections.Generic;
    public class UpdateMultipleOrder {

        public UpdateMultipleOrder(Tuple<string, object> id, string tableName, IEnumerable<Tuple<string, object>> values) {
            Id = id ?? Tuple.Create<string, object>("", null);
            TableName = tableName ?? "";
            Values = values ?? new Tuple<string, object>[0];
        }

        /// <summary>
        /// Identity field for table. Column name, and value.
        /// </summary>
        public Tuple<string, object> Id { get; set; }
        /// <summary>
        /// Table to update.
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// Column name, and value tuples.
        /// </summary>
        public IEnumerable<Tuple<string, object>> Values { get; set; }
    }

}
