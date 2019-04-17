namespace CSharpToolkit.Extensions {
    using CSharpToolkit.DataAccess.Abstractions;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Utilities;

    public static class WebRequestorExtensions {

        public static async Task<OperationResult<byte[]>> MakeWebRequestAsync(this IWebDataRequestor requestor, string url, IEnumerable<Tuple<string, string>> data, IEnumerable<Tuple<System.Net.HttpRequestHeader, string>> requestHeaders) =>
            await Task.Run(() => requestor.MakeWebRequest(url, data, requestHeaders));

    }
}
