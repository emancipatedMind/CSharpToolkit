namespace CSharpToolkit.DataAccess.Abstractions {
    using Utilities;
    public interface IReadable {
        OperationResult<byte[]> Read();
    }
}