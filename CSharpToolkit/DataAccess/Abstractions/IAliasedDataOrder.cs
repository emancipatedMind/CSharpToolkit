namespace CSharpToolkit.DataAccess.Abstractions {
    using Abstractions;
    using System;
    using System.Collections.Generic;
    using Utilities.Abstractions;
    /// <summary>
    /// Implemented by class who represents a data order containing a query, its parameters, and its aliases.
    /// </summary>
    public interface IAliasedDataOrder : ISimpleDataOrder {
        /// <summary>
        /// The aliases in use by Query.
        /// </summary>
        IEnumerable<IAlias> Aliases { get; }
    }
}