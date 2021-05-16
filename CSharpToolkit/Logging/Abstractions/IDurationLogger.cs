namespace CSharpToolkit.Logging.Abstractions {
    using System;
    /// <summary>
    /// Implemented by class who may log the duration of an operation.
    /// </summary>
    [Obsolete]
    public interface IDurationLogger : ILogger {
        /// <summary>
        /// Log time metrics.
        /// </summary>
        /// <param name="operation">Operation to log time metrics of.</param>
        void LogTimeMetricsOf(Action operation);
    }
}