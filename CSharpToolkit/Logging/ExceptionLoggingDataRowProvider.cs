namespace CSharpToolkit.Logging {
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Abstractions;
    using DataAccess.Abstractions;
    using Utilities;
    using System.Linq;

    /// <summary>
    /// Decorator for the IDataRowProvider that will log exceptions.
    /// </summary>
    public class ExceptionLoggingDataRowProvider : IDataRowProvider, IFileNameSwappable {

        IDataRowProvider _component;
        ITimeStampLogger _logger;
        IExceptionFormatter _exceptionFormatter;
        IFileNameSwappable _fileNameSwappableLogger;

        /// <summary>
        /// File name where logging will occur.
        /// </summary>
        public string FileName { set { _fileNameSwappableLogger.FileName = value; } }

        /// <summary>
        /// Instantiates LoggingDataRowProvider which will submit query to component, and if exception is encountered, it will be logged.
        /// Use ExceptionLoggingToBegin to set filename where logging will be set by casting sender to CSharpToolkit.Abstractions.Logging.IFileNameSwappable.
        /// </summary>
        /// <param name="component">Component to submit query to.</param>
        public ExceptionLoggingDataRowProvider(IDataRowProvider component) : this(component, new Logger(), new ExceptionFormatter()) { }

        /// <summary>
        /// Instantiates LoggingDataRowProvider which will submit query to component, and if exception is encountered, it will be logged.
        /// Use ExceptionLoggingToBegin to set filename where logging will be set by casting sender to CSharpToolkit.Abstractions.Logging.IFileNameSwappable.
        /// </summary>
        /// <param name="component">Component to submit query to.</param>
        /// <param name="logger">Logger for information, and exceptions if operation faulted.</param>
        public ExceptionLoggingDataRowProvider(IDataRowProvider component, IFileNameSwappableLogger logger) : this(component, logger, new ExceptionFormatter())  { }

        /// <summary>
        /// Instantiates LoggingDataRowProvider which will submit query to component, and if exception is encountered, it will be logged.
        /// Use ExceptionLoggingToBegin to set filename where logging will be set by casting sender to CSharpToolkit.Abstractions.Logging.IFileNameSwappable.
        /// </summary>
        /// <param name="component">Component to submit query to.</param>
        /// <param name="logger">Logger for information, and exceptions if operation faulted.</param>
        /// <param name="formatter">Formatter for exceptions.</param>
        public ExceptionLoggingDataRowProvider(IDataRowProvider component, IFileNameSwappableLogger logger, IExceptionFormatter formatter) {
            _component = component;
            _fileNameSwappableLogger = logger;
            _logger = new TimeStampLogger(logger);
            _exceptionFormatter = formatter;
        }

        /// <summary>
        /// Wrapper for component's SubmitQuery method will examine OperationResult and log if error found.
        /// </summary>
        /// <param name="sql">Query. Will be logged if exception thrown.</param>
        /// <param name="commandType">Command type of query. Will be logged if exception thrown.</param>
        /// <param name="parameters">Parameters needed by query. Will be logged if exception thrown.</param>
        /// <returns>Operation result.</returns>
        public OperationResult<List<DataRow>> SubmitQuery(string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters) {
            OperationResult<List<DataRow>> result = _component.SubmitQuery(sql, commandType, parameters);

            if (result.HadErrors) {
                var errorString =
                    Use.StringBuilder(builder => {
                        builder.AppendLine("Error encountered with Query :");
                        builder.AppendLine($"{sql}");
                        builder.AppendLine();
                        builder.AppendLine($"Command type : {commandType}");
                        if (parameters.Any()) {
                            builder.AppendLine($"\r\nParameters =>");
                            foreach (var parameter in parameters)
                                builder.AppendLine($"\t{parameter.Key} = {parameter.Value}");
                        }
                        foreach(var ex in result.Exceptions)
                            builder.AppendLine(_exceptionFormatter.FormatException(ex));
                    });

                ExceptionLoggingToBegin?.Invoke(this, EventArgs.Empty);
                _logger.LogWithCurrentTime(errorString, " : ", true);
                ExceptionLoggingComplete?.Invoke(this, EventArgs.Empty);
            }

            return result;
        }

        /// <summary>
        /// Denotes that exception logging has been completed.
        /// </summary>
        public event EventHandler ExceptionLoggingComplete;

        /// <summary>
        /// Denotes that exception logging is about to begin.
        /// </summary>
        public event EventHandler ExceptionLoggingToBegin;

    }
}