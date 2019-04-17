namespace CSharpToolkit.DataAccess.Abstractions {
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Utilities;
    /// <summary>
    /// Adorned by a class who can perform a requestor.
    /// </summary>
    public interface IWebDataRequestorAsync {
        /// <summary>
        /// Used to make a web request asynchronously.
        /// </summary>
        /// <param name="url">The URL for request.</param>
        /// <param name="data">The data for the request.</param>
        /// <param name="requestHeaders">The headers for the call.</param>
        /// <returns>An <see cref="OperationResult"/>  containing the bytes returned from the web call.</returns>
        Task<OperationResult<byte[]>> MakeWebRequestAsync(string url, IEnumerable<Tuple<string, string>> data, IEnumerable<Tuple<System.Net.HttpRequestHeader, string>> requestHeaders);
    }
}