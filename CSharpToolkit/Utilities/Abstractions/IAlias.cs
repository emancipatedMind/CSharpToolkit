namespace CSharpToolkit.Utilities.Abstractions {
    public interface IAlias {
        string Name { get; }
        System.Tuple<string, string> Data { get; }
    }
}