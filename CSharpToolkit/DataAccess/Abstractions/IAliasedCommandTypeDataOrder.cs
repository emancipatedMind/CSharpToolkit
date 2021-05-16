namespace CSharpToolkit.DataAccess.Abstractions {
    using System.Data;
    /// <summary>
    /// Implemented by class who represents a data order containing a query, its parameters, its aliases, and its command type.
    /// </summary>
    public interface IAliasedCommandTypeDataOrder : IAliasedDataOrder {
        /// <summary>
        /// The CommandType of the Query.
        /// </summary>
        CommandType CommandType { get; }
    }
}