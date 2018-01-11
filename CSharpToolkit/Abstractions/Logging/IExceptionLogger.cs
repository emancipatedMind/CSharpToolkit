namespace CSharpToolkit.Abstractions.Logging {
    using System;
    /// <summary>
    /// Implemented by class who may log exceptions.
    /// </summary>
    public interface IExceptionLogger : IFileNameSwappableLogger {
        /// <summary>
        /// Log Exception.
        /// </summary>
        /// <param name="ex">Exception to be logged.</param>
        void LogException(Exception ex);
    }
}
