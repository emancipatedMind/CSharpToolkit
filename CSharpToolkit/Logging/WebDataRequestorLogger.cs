namespace CSharpToolkit.Logging {
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Linq;
    using DataAccess.Abstractions;
    using Utilities;
    using Utilities.Abstractions;

    public class WebDataRequestorLogger : IWebDataRequestor {

        byte[] _response;
        Logger _logger = new Logger(); 

        public OperationResult<byte[]> MakeWebRequest(string url, IEnumerable<Tuple<string, string>> data, IEnumerable<Tuple<HttpRequestHeader, string>> requestHeaders) {
            WebLoggingToBegin?.Invoke(this, new Utilities.EventArgs.GenericEventArgs<IFileNameSwappable>(_logger));

            string webRequest = Use.StringBuilder(builder => {
                builder.AppendLine("***** WEB REQUEST *****");
                builder.AppendLine();
                builder.AppendLine($"URL : {url}");
                if (data.Any()) {
                    builder.AppendLine($"Data :");
                    builder.Append(string.Join("\r\n", data.Where(d => string.IsNullOrWhiteSpace(d.Item2) == false).Select(d => $"\t{d.Item1} :\r\n\t\t{string.Join("\\r\\n", d.Item2.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n'))}")));
                }
                if (requestHeaders.Any()) {
                    builder.AppendLine($"Request Headers :");
                    builder.Append(string.Join("\r\n", requestHeaders.Select(d => $"\t{d.Item1} :\r\n\t\t{d.Item2}")));
                }
                builder.AppendLine();
                builder.AppendLine("***** END *****");
            });

            OperationResult loggingOperation =
                _logger.Log(webRequest);
            if (loggingOperation.WasSuccessful) {
                WebLoggingCompletedSuccessfully?.Invoke(this, new Utilities.EventArgs.GenericEventArgs<string>(_logger.FileName));
                return new OperationResult<byte[]>(loggingOperation.Exceptions);
            }
            return new OperationResult<byte[]>(Response);
        }

        public event EventHandler<Utilities.EventArgs.GenericEventArgs<string>> WebLoggingCompletedSuccessfully;
        public event EventHandler<Utilities.EventArgs.GenericEventArgs<IFileNameSwappable>> WebLoggingToBegin;
        public byte[] Response { get { return _response ?? new byte[0]; } set { _response = value; } }

    }
}