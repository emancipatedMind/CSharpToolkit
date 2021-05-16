namespace CSharpToolkit.DataAccess {
    using Abstractions;
    using System.Collections.Generic;
    using System.Data;
    using Utilities.Abstractions;
    /// <summary>
    /// Represents a data order containing a query, its parameters, its aliases, and its command type.
    /// </summary>
    public class AliasedCommandTypeDataOrder : AliasedDataOrder, IAliasedCommandTypeDataOrder {

        /// <summary>
        /// Creates empty data order.
        /// </summary>
        public AliasedCommandTypeDataOrder() : this("", CommandType.Text, new KeyValuePair<string, object>[0], new IAlias[0]) { }

        /// <summary>
        /// Creates data order. Allows a previously created order's CommandType to be specified. 
        /// </summary>
        /// <param name="order">Previously built data order.</param>
        /// <param name="commandType">The commandType of the Query.</param>
        public AliasedCommandTypeDataOrder(ISimpleDataOrder order, CommandType commandType) : this(order.Query, commandType, order.Parameters, new IAlias[0]) { }

        /// <summary>
        /// Creates data order. Allows a previously created order's CommandType to be specified. 
        /// </summary>
        /// <param name="order">Previously built data order.</param>
        /// <param name="commandType">The commandType of the Query.</param>
        public AliasedCommandTypeDataOrder(IAliasedDataOrder order, CommandType commandType) : this(order.Query, commandType, order.Parameters, order.Aliases) { }

        /// <summary>
        /// Creates data order along with parameters.
        /// </summary>
        /// <param name="query">The built query.</param>
        /// <param name="commandType">The commandType of the Query.</param>
        /// <param name="parameters">The parameters needed by Query.</param>
        public AliasedCommandTypeDataOrder(string query, CommandType commandType, IEnumerable<KeyValuePair<string, object>>parameters) : this(query, commandType, parameters, new IAlias[0]) { }

        /// <summary>
        /// Creates data order with no parameters or aliases.
        /// </summary>
        /// <param name="query">The built query.</param>
        /// <param name="commandType">The commandType of the Query.</param>
        public AliasedCommandTypeDataOrder(string query, CommandType commandType) : this(query, commandType, new KeyValuePair<string, object>[0], new IAlias[0]) { }

        /// <summary>
        /// Creates data order along with parameters, and aliases.
        /// </summary>
        /// <param name="query">The built query.</param>
        /// <param name="commandType">The commandType of the Query.</param>
        /// <param name="parameters">The parameters needed by Query.</param>
        /// <param name="aliases">The aliases in use by Query.</param>
        public AliasedCommandTypeDataOrder(string query, CommandType commandType, IEnumerable<KeyValuePair<string, object>>parameters, IEnumerable<IAlias> aliases) : base(query, parameters, aliases)  {
            CommandType = commandType;
        }

        /// <summary>
        /// The CommandType of the Query.
        /// </summary>
        public CommandType CommandType { get; }
    }
}