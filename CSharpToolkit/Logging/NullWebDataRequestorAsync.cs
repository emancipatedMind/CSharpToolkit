namespace CSharpToolkit.Logging {
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using DataAccess.Abstractions;
    using Utilities;
    /// <summary>
    /// An implementation of the <see cref="IWebDataRequestorAsync"/> who returns faulted <see cref="OperationResult"/>.
    /// </summary>
    public class NullWebDataRequestorAsync : IWebDataRequestorAsync {

        static NullWebDataRequestorAsync _instance;
        /// <summary>
        /// An implementation of the singleton pattern for the <see cref="NullWebDataRequestorAsync"/> class.
        /// </summary>
        public static NullWebDataRequestorAsync Instance => _instance ?? (_instance = new NullWebDataRequestorAsync());

        NullWebDataRequestorAsync() { }

        /// <summary>
        /// Returns a completed <see cref="Task"/> whose <see cref="OperationResult"/> is faulted with a <see cref="NotImplementedException"/>.
        /// </summary>
        /// <param name="url">The URL for request.</param>
        /// <param name="data">The data for the request.</param>
        /// <param name="requestHeaders">The headers for the call.</param>
        /// <returns>A faulted <see cref="OperationResult"/> with <see cref="NotImplementedException"/>.</returns>
        public Task<OperationResult<byte[]>> MakeWebRequestAsync(string url, IEnumerable<Tuple<string, string>> data, IEnumerable<Tuple<HttpRequestHeader, string>> requestHeaders) =>
            Task.FromResult(new OperationResult<byte[]>(new[] { new NotImplementedException() }));
    }
}