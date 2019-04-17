namespace CSharpToolkit.Utilities.Abstractions {
    using System.Threading.Tasks;
    public interface IStream : System.IDisposable {
        Task<OperationResult<byte[]>> QueryStream(byte[] query);
        int QueryDelay { set; get; }
        byte[] Terminator { set; get; }

    }
}