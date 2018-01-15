namespace CSharpToolkit.DataAccess.Abstractions {
    using Abstractions;
    using System;
    using System.Collections.Generic;
    using Utilities.Abstractions;
    public interface IAliasedDataOrder : ISimpleDataOrder {
        IEnumerable<IAlias> Aliases { get; }
    }
}