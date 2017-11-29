namespace CSharpToolkit.Abstractions.Logging {
    using System;
    public interface IDurationLogger : ILogger {
        void LogTimeMetricsOf(Action operation);
    }
}