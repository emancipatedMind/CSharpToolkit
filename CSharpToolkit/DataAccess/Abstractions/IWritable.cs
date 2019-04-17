namespace CSharpToolkit.DataAccess.Abstractions {
    using Utilities;
    public interface IWritable {
        OperationResult Write(byte[] message);
    }
}