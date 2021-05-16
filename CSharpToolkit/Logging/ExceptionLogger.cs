namespace CSharpToolkit.Logging {
    using Abstractions;
    using System;
    using Utilities;
    using Utilities.Abstractions;
    using Utilities.EventArgs;

    /// <summary>
    /// Logs exceptions.
    /// </summary>
    public class ExceptionLogger : IExceptionLogger, IFileNameSwappable {

        IFileNameSwappableLogger _logger;
        IExceptionFormatter _formatter;

        /// <summary>
        /// File name where exception is logged.
        /// </summary>
        public string FileName {
            get { return _logger.FileName; }
            set { _logger.FileName = value; }
        }

        /// <summary>
        /// Instantiates ExceptionLogger.
        /// </summary>
        public ExceptionLogger() : this(new Logger(), new ExceptionFormatter()) { }
        /// <summary>
        /// Instantiates ExceptionLogger.
        /// </summary>
        /// <param name="logger">Logger used to log exceptions.</param>
        public ExceptionLogger(IFileNameSwappableLogger logger) : this(logger, new ExceptionFormatter()) { }
        /// <summary>
        /// Instantiates ExceptionLogger.
        /// </summary>
        /// <param name="logger">Logger used to log exceptions.</param>
        /// <param name="formatter">Formatter used for Exceptions.</param>
        public ExceptionLogger(IFileNameSwappableLogger logger, IExceptionFormatter formatter) {
            _formatter = formatter;
            _logger = logger;
        }

        /// <summary>
        /// Log Exceptions.
        /// </summary>
        /// <param name="exceptions">Exception to be logged.</param>
        /// <returns>Operation result detailing whether log was successful.</returns>
        public OperationResult LogExceptions(params Exception[] exceptions) {
            var operation = Get.OperationResult(() => _logger.Log(_formatter.FormatException(exceptions)));

            ErrorLogged?.Invoke(this, new GenericEventArgs<bool>(operation.WasSuccessful));
            return operation;
        }

        /// <summary>
        /// Fires after the error has been logged denoting whether log was successful.
        /// </summary>
        public event EventHandler<GenericEventArgs<bool>> ErrorLogged;

    }
}