namespace CSharpToolkit.Logging {
    using System;
    using Abstractions.Logging;
    using EventArgs;
    /// <summary>
    /// Logs exceptions.
    /// </summary>
    public class ExceptionLogger : IExceptionLogger {

        IFileNameSwappableLogger _internalLogger;
        IExceptionFormatter _formatter;

        /// <summary>
        /// Instantiates ExceptionLogger.
        /// </summary>
        /// <param name="internalLogger">Logger used to log exceptions.</param>
        /// <param name="formatter">Formatter used for Exceptions.</param>
        public ExceptionLogger(IFileNameSwappableLogger internalLogger, IExceptionFormatter formatter) {
            _formatter = formatter;
            _internalLogger = internalLogger;
            _internalLogger.LogOutputFailure += (s, e) => LogOutputFailure?.Invoke(this, e);
        } 

        /// <summary>
        /// File name for exception logging.
        /// </summary>
        public string FileName { set { _internalLogger.FileName = value; } }

        /// <summary>
        /// Indicates whether Logger has faulted.
        /// </summary>
        public bool LoggerFaulted => _internalLogger.LoggerFaulted;

        /// <summary>
        /// Raied to indicate logger has failed to output to destination.
        /// </summary>
        public event EventHandler<GenericEventArgs<Exception>> LogOutputFailure;

        /// <summary>
        /// Log Content.
        /// </summary>
        /// <param name="content">Content to be logged.</param>
        public void Log(string content) => _internalLogger.Log(content);

        /// <summary>
        /// Log Exception.
        /// </summary>
        /// <param name="ex">Exception to be logged.</param>
        public void LogException(Exception ex) => Log(_formatter.FormatException(ex)); 

    }
}
