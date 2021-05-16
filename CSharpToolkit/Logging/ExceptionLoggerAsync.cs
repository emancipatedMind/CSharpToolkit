namespace CSharpToolkit.Logging {
    using System;
    using System.Threading.Tasks;
    using Abstractions;
    using Utilities;
    using Utilities.Abstractions;
    /// <summary>
    /// Log Exceptions Asynchronously.
    /// </summary>
    public class ExceptionLoggerAsync : IExceptionLoggerAsync, IFileNameSwappable {

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
        public ExceptionLoggerAsync() : this(new Logger(), new ExceptionFormatter()) { }
        /// <summary>
        /// Instantiates ExceptionLogger.
        /// </summary>
        /// <param name="logger">Logger used to log exceptions.</param>
        public ExceptionLoggerAsync(IFileNameSwappableLogger logger) : this(logger, new ExceptionFormatter()) { }
        /// <summary>
        /// Instantiates ExceptionLogger.
        /// </summary>
        /// <param name="logger">Logger used to log exceptions.</param>
        /// <param name="formatter">Formatter used for Exceptions.</param>
        public ExceptionLoggerAsync(IFileNameSwappableLogger logger, IExceptionFormatter formatter) {
            _formatter = formatter;
            _logger = logger;
        }

        /// <summary>
        /// Log Exceptions Asynchronously.
        /// </summary>
        /// <param name="exceptions">Exception to be logged.</param>
        /// <returns>Operation result detailing whether log was successful.</returns>
        public async Task<OperationResult> LogExceptionsAsync(params Exception[] exceptions) =>
            await Get.OperationResultAsync(() => _logger.Log(_formatter.FormatException(exceptions)));

    }
}