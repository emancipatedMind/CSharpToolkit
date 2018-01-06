
namespace CSharpToolkit.Abstractions.DataAccess {
    using System.Collections.Generic;
    public interface ISimpleDataOrder {
        string Query { get; }
        IEnumerable<KeyValuePair<string, object>> Parameters { get; }
    }
}