namespace CSharpToolkit.Logging {
    using System;
    using System.IO;
    using Abstractions.Logging;
    using EventArgs;
    /// <summary>
    /// Generic logger whose file name may be swapped.
    /// </summary>
    public class Logger : IFileNameSwappableLogger {

        string _fileName;

        /// <summary>
        /// Instantiates Logger. Must pass in FileName to log.
        /// </summary>
        public Logger() : this("") { }
        /// <summary>
        /// Instantiates Logger.
        /// </summary>
        /// <param name="logFile">File where logging is done.</param>
        public Logger(string logFile) {
            _fileName = logFile;
        }

        /// <summary>
        /// File name for logging. Changing file name can reset logger faulted status.
        /// </summary>
        public string FileName {
            set {
                if (_fileName == value) return; 
                _fileName = value;
                LoggerFaulted = false;
            }
        }

        /// <summary>
        /// Raied to indicate logger has failed to output to destination.
        /// </summary>
        public event EventHandler<GenericEventArgs<Exception>> LogOutputFailure;

        /// <summary>
        /// Indicates whether Logger has faulted.
        /// </summary>
        public bool LoggerFaulted { get; private set; } = false;

        /// <summary>
        /// Log Content.
        /// </summary>
        /// <param name="content">Content to be logged.</param>
        public void Log(string content) {
            if (LoggerFaulted) return;
            try {
                using (var writer = File.AppendText(_fileName))
                    writer.Write(content);
            }
            catch(Exception ex) {
                LoggerFaulted = true;
                LogOutputFailure?.Invoke(this, new GenericEventArgs<Exception>(ex));
            }
        }

    }
}
