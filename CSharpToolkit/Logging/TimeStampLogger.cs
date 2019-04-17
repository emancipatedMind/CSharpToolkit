namespace CSharpToolkit.Logging {
    using Abstractions;
    using System;
    using Utilities;
    /// <summary>
    /// Logs objects with time stamps.
    /// </summary>
    public class TimeStampLogger : ILogger {

        const string DATETIMEDEFAULTFORMAT = "MM/dd/yyyy HH:mm:ss.fff";

        string _dateTimeFormat;
        string _prependageText;
        ILogger _logger;

        /// <summary>
        /// Instantiates TimeStampLogger.
        /// </summary>
        /// <param name="fileName">File where to log.</param>
        public TimeStampLogger(string fileName) : this(new Logger(fileName)) { } 

        /// <summary>
        /// Instantiates TimeStampLogger.
        /// </summary>
        /// <param name="logger">Logger to use.</param>
        public TimeStampLogger(ILogger logger) {
            _logger = logger;
        }

        /// <summary>
        /// Text to prepend to line before time.
        /// </summary>
        public string LinePrependageText {
            get { return _prependageText ?? ""; }
            set { _prependageText = value; }
        }

        /// <summary>
        /// The DateTime format to use. "MM/dd/yyyy HH:mm:ss.fff" is default.
        /// </summary>
        public string DateTimeFormat { get {return _dateTimeFormat ?? DATETIMEDEFAULTFORMAT; } set { _dateTimeFormat = value; } }

        /// <summary>
        /// Logs current time.
        /// </summary>
        /// <param name="content">Content to log.</param>
        /// <returns>Operation result detailing whether log was successful.</returns>
        public OperationResult Log(object content) =>
            _logger.Log($"{LinePrependageText}{DateTime.Now.ToString(DateTimeFormat, System.Globalization.CultureInfo.InvariantCulture)} : {content}");
    }
}