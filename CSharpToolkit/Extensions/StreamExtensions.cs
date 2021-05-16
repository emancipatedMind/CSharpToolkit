namespace CSharpToolkit.Extensions {
    using System.Threading.Tasks;
    using System.Text;
    using System.Linq;
    using Utilities;
    using Utilities.Abstractions;
    public static class StreamExtensions {

        public static async Task<OperationResult<string>> QueryStream(this IStream stream, string request) {
            byte[] rawQuery = Encoding.ASCII.GetBytes(request);
            OperationResult<byte[]> response = await stream.QueryStream(rawQuery);
            if (response.WasSuccessful == false)
                return new OperationResult<string>(response.Exceptions);
            return new OperationResult<string>(Encoding.ASCII.GetString(response.Result, 0, response.Result.Length));
        }

    }
}