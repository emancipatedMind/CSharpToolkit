namespace CSharpToolkit.Logging {
    using System;
    using Abstractions;
    using Utilities;
    /// <summary>
    /// Logs the duration of an operation.
    /// </summary>
    public class DurationLogger {

        ILogger _logger;

        /// <summary>
        /// Instantiates DurationLogger.
        /// </summary>
        /// <param name="logger">Logger to use.</param>
        public DurationLogger(ILogger logger) {
            _logger = logger;
        }

        /// <summary>
        /// Instantiates DurationLogger.
        /// </summary>
        /// <param name="fileName">File name where to log.</param>
        public DurationLogger(string fileName) : this(new Logger(fileName)) { }

        /// <summary>
        /// Log time metrics.
        /// </summary>
        /// <param name="operation">Operation to log time metrics of.</param>
        /// <returns>Operation result detailing whether log was successful.</returns>
        public OperationResult LogTimeMetricsOf(Action operation) {
            string format = "MM/dd/yyyy HH:mm:ss.fffffff";
            DateTime beginTime = DateTime.Now;
            operation();
            DateTime finishTime = DateTime.Now;
            var messages = new string[] {
                $"Operation begin time : {beginTime.ToString(format) }",
                $"Operation end time : {finishTime.ToString(format) }",
                $"Operation Duration : {(finishTime - beginTime).TotalMilliseconds} ms.\r\n",
            };
            return _logger.Log(string.Join("\r\n", messages));
        }
    }
}