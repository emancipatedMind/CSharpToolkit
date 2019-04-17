namespace CSharpToolkit.Extensions {
    using System.Threading.Tasks;
    using DataAccess.Abstractions;
    using Utilities;
    public static class CommunicatorExtensions {

        public static OperationResult WriteString(this ICommunicator communicator, string request) {
            if (string.IsNullOrEmpty(request))
                return new OperationResult(false);
            byte[] rawRequest = System.Text.Encoding.ASCII.GetBytes(request);
            return communicator.Write(rawRequest);
        }

        public static OperationResult<string> ReadString(this ICommunicator communicator) {
            OperationResult<byte[]> rawResponse = communicator.Read();
            return
                rawResponse.WasSuccessful ?
                    new OperationResult<string>(System.Text.Encoding.ASCII.GetString(rawResponse.Result)) :
                    new OperationResult<string>(rawResponse.Exceptions);
        }

        public static OperationResult<string> QueryString(this ICommunicator communicator, string request, int readDelay) {
            OperationResult writeOperation = communicator.WriteString(request);
            if (writeOperation.WasSuccessful == false)
                return new OperationResult<string>(writeOperation.Exceptions);

            System.Threading.Thread.Sleep(readDelay);

            return communicator.ReadString();
        }

        public static Task<OperationResult<string>> ReadStringAsync(this ICommunicator communicator) =>
            Task.Run(() => communicator.ReadString());

        public static Task<OperationResult> WriteStringAsync(this ICommunicator communicator, string request) =>
            Task.Run(() => communicator.WriteString(request));

        public static Task<OperationResult<string>> QueryStringAsync(this ICommunicator communicator, string request, int readDelay) =>
            Task.Run(() => communicator.QueryString(request, readDelay));

    }
}