namespace CSharpToolkit.Abstractions.DataAccess {
    using System.Collections.Generic;
    /// <summary>
    /// Implemented by class who represents a simple data order containing a query, and its parameters.
    /// </summary>
    public interface ISimpleDataOrder {
        /// <summary>
        /// The built query.
        /// </summary>
        string Query { get; }
        /// <summary>
        /// The parameters needed by query.
        /// </summary>
        IEnumerable<KeyValuePair<string, object>> Parameters { get; }
    }
}