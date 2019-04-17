namespace CSharpToolkit.DataAccess.Web {
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Linq;
    using System.Threading.Tasks;
    using Utilities;
    using Extensions;
    /// <summary>
    /// Makes a POST web call using the ContentType of "application/x-www-form-urlencoded". Combines data in following form : key1=value1&amp;key1=value2&amp;key3=value3.
    /// </summary>
    public class PostAsyncWebClient : Abstractions.IWebDataRequestorAsync {
        /// <summary>
        /// Makes a POST web call asynchronously using the ContentType of "application/x-www-form-urlencoded". Combines data in following form : var1=val1&amp;var2=val2&amp;var3=val3
        /// </summary>
        /// <param name="url">The URL to which the call is made.</param>
        /// <param name="data">The pairs of data to transfer. Item1 is key, and Item2 is value.</param>
        /// <param name="requestHeaders">The request headers for the call. If ContentType is specified among these, it is ignored.</param>
        /// <returns>Operation result containing response.</returns>
        public async Task<OperationResult<byte[]>> MakeWebRequestAsync(string url, IEnumerable<Tuple<string, string>> data, IEnumerable<Tuple<HttpRequestHeader, string>> requestHeaders = null) {
            data = data ?? new Tuple<string, string>[0];
            requestHeaders = requestHeaders ?? new Tuple<HttpRequestHeader, string>[0];

            byte[] postData = System.Text.Encoding.ASCII.GetBytes(string.Join("&", data.Select(d => $"{EscapeProblemCharacters(d.Item1)}={EscapeProblemCharacters(d.Item2)}")));

            try {
                using (var client = new WebClient()) {
                    requestHeaders
                        .Where(h => h.Item1 != HttpRequestHeader.ContentType)
                        .Concat(new[] { new Tuple<HttpRequestHeader, string>(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded") })
                        .ForEach(h => client.Headers.Add(h.Item1, h.Item2));

                    byte[] response = await client.UploadDataTaskAsync(url, postData);
                    return new OperationResult<byte[]>(response);
                }
            }
            catch (Exception ex) {
                return new OperationResult<byte[]>(new[] { ex });
            }
        }

        private static string EscapeProblemCharacters(string input) =>
            string.IsNullOrEmpty(input) ? "" : input.Replace("&", "%26").Replace("=", "%2D");

    }
}