namespace CSharpToolkit.DataAccess.Web {
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Linq;
    using System.Threading.Tasks;
    using Extensions;
    using Utilities;
    /// <summary>
    /// Makes a GET web call. Combines data in following form : var1=val1&amp;var2=val2&amp;var3=val3. This is appended to url with a question mark.
    /// </summary>
    public class GetAsyncWebClient : Abstractions.IWebDataRequestorAsync {

        /// <summary>
        /// Makes a GET web call asynchronously. Combines data in following form : key1=value1&amp;key2=value2&amp;key3=value3. This is appended to url with a question mark.
        /// </summary>
        /// <param name="url">The URL to which the call is made.</param>
        /// <param name="data">The pairs of data to transfer. Item1 is key, and Item2 is value.</param>
        /// <param name="requestHeaders">The request headers for the call.</param>
        /// <returns>Operation result containing response.</returns>
        public async Task<OperationResult<byte[]>> MakeWebRequestAsync(string url, IEnumerable<Tuple<string, string>> data, IEnumerable<Tuple<HttpRequestHeader, string>> requestHeaders = null) {
            data = data ?? new Tuple<string, string>[0];
            requestHeaders = requestHeaders ?? new Tuple<HttpRequestHeader, string>[0];

            string finishedUrl = $"{url}?{string.Join("&", data.Select(d => $"{d.Item1}={d.Item2}"))}";

            try {
                using (var client = new WebClient()) {
                    requestHeaders
                        .ForEach(h => client.Headers.Add(h.Item1, h.Item2));
                    byte[] response = await client.DownloadDataTaskAsync(finishedUrl);
                    return new OperationResult<byte[]>(response);
                }
            }
            catch (Exception ex) {
                return new OperationResult<byte[]>(new[] { ex });
            }
        }
    }
}