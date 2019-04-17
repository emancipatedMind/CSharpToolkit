namespace CSharpToolkit.DataAccess {
    using Abstractions;
    using System.Collections.Generic;
    using Utilities.Abstractions;
    /// <summary>
    /// Represents a data order containing a query, its parameters, and its aliases.
    /// </summary>
    public class AliasedDataOrder : SimpleDataOrder, IAliasedDataOrder {

        /// <summary>
        /// Create aliased data order. This allows you include aliases along with your data order.
        /// </summary>
        public AliasedDataOrder() : this("", new KeyValuePair<string, object>[0], new IAlias[0]) { }

        /// <summary>
        /// Create aliased data order. This allows you include aliases along with your data order.
        /// </summary>
        /// <param name="order">Previously built data order.</param>
        public AliasedDataOrder(ISimpleDataOrder order) : this(order.Query, order.Parameters, new IAlias[0]) { }

        /// <summary>
        /// Create aliased data order. This allows you include aliases along with your data order.
        /// </summary>
        /// <param name="order">Previously built data order.</param>
        /// <param name="aliases">The aliases in use by Query.</param>
        public AliasedDataOrder(ISimpleDataOrder order, IEnumerable<IAlias> aliases) : this(order.Query, order.Parameters, aliases) { }

        /// <summary>
        /// Create aliased data order. This allows you include aliases along with your data order.
        /// </summary>
        /// <param name="query">The built query.</param>
        /// <param name="parameters">The parameters needed by Query.</param>
        public AliasedDataOrder(string query, IEnumerable<KeyValuePair<string, object>> parameters) : this(query, parameters, new IAlias[0]) { }

        /// <summary>
        /// Create aliased data order. This allows you include aliases along with your data order.
        /// </summary>
        /// <param name="query">The built query.</param>
        /// <param name="aliases">The aliases in use by Query.</param>
        public AliasedDataOrder(string query, IEnumerable<IAlias> aliases) : this(query, new KeyValuePair<string, object>[0], aliases) { }

        /// <summary>
        /// Create aliased data order. This allows you include aliases along with your data order.
        /// </summary>
        /// <param name="query">The built query.</param>
        /// <param name="parameters">The parameters needed by Query.</param>
        /// <param name="aliases">The aliases in use by Query.</param>
        public AliasedDataOrder(string query, IEnumerable<KeyValuePair<string, object>> parameters, IEnumerable<IAlias> aliases) : base(query, parameters)  {
            Aliases = aliases;
        }

        /// <summary>
        /// The aliases in use by Query.
        /// </summary>
        public IEnumerable<IAlias>Aliases { get; }
    }
}