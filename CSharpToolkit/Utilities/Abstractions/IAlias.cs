namespace CSharpToolkit.Utilities.Abstractions {
    using System.Collections.Generic;
    public interface IAlias {
        string Name { get; }
        KeyValuePair<string, string> Data { get; }
    }
}