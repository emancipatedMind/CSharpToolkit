namespace CSharpToolkit.Logging {
    using System;
    using System.Data;
    using DataAccess.Abstractions;
    using Logging.Abstractions;
    using EventArgs;

    /// <summary>
    /// Logs the duration of an operation.
    /// </summary>
    public class DurationLogger : IDurationLogger {

        ITimeStampLogger _logger;
        string _separationString = " : " ;

        /// <summary>
        /// Instantiates DurationLogger. Uses ITimeStampLogger to perform its logging.
        /// </summary>
        /// <param name="logger"></param>
        public DurationLogger(ITimeStampLogger logger) {
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
        /// Log time metrics.
        /// </summary>
        /// <param name="operation">Operation to log time metrics of.</param>
        public void LogTimeMetricsOf(Action operation) {
            DateTime beginTime = LogWithCurrentTime("Operation has begun.\r\n", false);
            operation();
            DateTime finishTime = LogWithCurrentTime("Operation Done.\r\n", false);
            LogWithCurrentTime($"Operation Duration : {(finishTime - beginTime).TotalMilliseconds} ms.\r\n", false);
            _logger.Log("\r\n");
        }

        DateTime LogWithCurrentTime(string content, bool prependNewLine) =>
            _logger.LogWithCurrentTime(content, _separationString, prependNewLine);

    }
}