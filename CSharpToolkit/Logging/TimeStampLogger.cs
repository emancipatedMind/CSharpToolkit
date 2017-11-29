namespace CSharpToolkit.Logging {
    using System;
    using Abstractions.Logging;
    using EventArgs;
    public class TimeStampLogger : ITimeStampLogger {

        ILogger _logger;

        public TimeStampLogger(ILogger logger) {
            _logger = logger;
            _logger.LogOutputFailure += (s, e) => LogOutputFailure?.Invoke(this, e);
        }

        public bool LoggerFaulted => _logger.LoggerFaulted;

        public event EventHandler<GenericEventArgs<Exception>> LogOutputFailure;

        public void Log(string content) => _logger.Log(content);

        public DateTime LogWithCurrentTime(string content, string dateSeparationString, bool prependNewLine) {
            var currentTime = DateTime.Now;
            Log($"{(prependNewLine ? "\r\n" : "")}{currentTime.ToString("MM/dd/yyyy hh:mm:ss tt")} {dateSeparationString}{content}");
            return currentTime;
        }
    }
}
