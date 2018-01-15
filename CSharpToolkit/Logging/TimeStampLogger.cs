namespace CSharpToolkit.Logging {
    using System;
    using Abstractions;
    using EventArgs;
    /// <summary>
    /// Logs time stamps.
    /// </summary>
    public class TimeStampLogger : ITimeStampLogger {

        ILogger _logger;

        public TimeStampLogger(ILogger logger) {
            _logger = logger;
            _logger.LogOutputFailure += (s, e) => LogOutputFailure?.Invoke(this, e);
        }

        /// <summary>
        /// Indicates whether Logger has faulted.
        /// </summary>
        public bool LoggerFaulted => _logger.LoggerFaulted;

        /// <summary>
        /// Raied to indicate logger has failed to output to destination.
        /// </summary>
        public event EventHandler<GenericEventArgs<Exception>> LogOutputFailure;

        /// <summary>
        /// Log Content.
        /// </summary>
        /// <param name="content">Content to be logged.</param>
        public void Log(string content) => _logger.Log(content);

        /// <summary>
        /// Logs current time.
        /// </summary>
        /// <param name="content">Content to log.</param>
        /// <param name="dateSeparationString">Token to separate content, and date.</param>
        /// <param name="prependNewLine">Whether a new line is prepended or not.</param>
        /// <returns>Time which was logged.</returns>
        public DateTime LogWithCurrentTime(string content, string dateSeparationString, bool prependNewLine) {
            var currentTime = DateTime.Now;
            Log($"{(prependNewLine ? "\r\n" : "")}{currentTime.ToString("MM/dd/yyyy hh:mm:ss tt")} {dateSeparationString}{content}");
            return currentTime;
        }
    }
}
