namespace CSharpToolkit.DataAccess {
    using System.Collections.Generic;
    using Abstractions.DataAccess;
    public class SimpleDataOrder : ISimpleDataOrder {
        public SimpleDataOrder(string query, IEnumerable<KeyValuePair<string, object>> parameters) {
            Query = query;
            Parameters = parameters;
        }

        public string Query { get; }
        public IEnumerable<KeyValuePair<string, object>> Parameters { get; }
    }
}