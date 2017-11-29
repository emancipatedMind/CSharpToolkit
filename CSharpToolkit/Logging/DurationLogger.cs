namespace CSharpToolkit.Logging {
    using System;
    using System.Data;
    using Abstractions.DataAccess;
    using Abstractions.Logging;
    using EventArgs;

    public class DurationLogger : IDurationLogger {

        ITimeStampLogger _logger;
        string _separationString = " : " ;

        public DurationLogger(ITimeStampLogger logger) {
            _logger = logger;
            _logger.LogOutputFailure += (s, e) => LogOutputFailure?.Invoke(this, e);
        }

        public bool LoggerFaulted => _logger.LoggerFaulted;

        public event EventHandler<GenericEventArgs<Exception>> LogOutputFailure;

        public void Log(string content) => _logger.Log(content);

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