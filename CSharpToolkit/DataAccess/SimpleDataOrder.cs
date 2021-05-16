namespace CSharpToolkit.DataAccess {
    using System.Collections.Generic;
    using Abstractions;
    /// <summary>
    /// Represents a data order containing a query, and its parameters.
    /// </summary>
    public class SimpleDataOrder : ISimpleDataOrder {

        /// <summary>
        /// Creates empty <see cref="SimpleDataOrder"/>.
        /// </summary>
        public SimpleDataOrder() : this (string.Empty, new KeyValuePair<string, object>[0]) { }

        /// <summary>
        /// Creates a data order containing a query.
        /// </summary>
        /// <param name="query">The query.</param>
        public SimpleDataOrder(string query) : this (query, new KeyValuePair<string, object>[0]) { }

        /// <summary>
        /// Creates a data order containing a query, and its parameters.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="parameters">The parameters inside of query.</param>
        public SimpleDataOrder(string query, IEnumerable<KeyValuePair<string, object>> parameters) {
            Query = query;
            Parameters = parameters;
        }

        /// <summary>
        /// The built query.
        /// </summary>
        public string Query { get; }
        /// <summary>
        /// The parameters needed by query.
        /// </summary>
        public IEnumerable<KeyValuePair<string, object>> Parameters { get; }
    }
}