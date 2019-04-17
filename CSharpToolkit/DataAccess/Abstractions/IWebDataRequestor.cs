using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpToolkit.DataAccess.Abstractions {
    using CSharpToolkit.Utilities;
    /// <summary>
    /// Adorned by a class who can perform a requestor.
    /// </summary>
    public interface IWebDataRequestor {
        /// <summary>
        /// Used to make a web request.
        /// </summary>
        /// <param name="url">The URL for request.</param>
        /// <param name="data">The data for the request.</param>
        /// <param name="requestHeaders">The headers for the call.</param>
        /// <returns>An operation result containing the bytes returned from the web call.</returns>
        OperationResult<byte[]> MakeWebRequest(string url, IEnumerable<Tuple<string, string>> data, IEnumerable<Tuple<System.Net.HttpRequestHeader, string>> requestHeaders);
    }
}