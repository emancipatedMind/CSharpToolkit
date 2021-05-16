namespace CSharpToolkit.DataAccess.Abstractions {
    public interface ICommunicatorProvider {
        ICommunicator OpenCommunicator();
    }
}