namespace CSharpToolkit.Logging {
    using Abstractions;
    using System;
    using System.IO;
    using Utilities;
    /// <summary>
    /// Logger for logging to files with the ability to swap the file name. Uses dual lock in case of multi-thread logging.
    /// </summary>
    public class Logger : IFileNameSwappableLogger {

        object _firstLockToken = new object();
        object _secondLockToken = new object();

        /// <summary>
        /// Instantiates Logger. FileName must be specified before use.
        /// </summary>
        public Logger() : this("") { }

        /// <summary>
        /// Instantiates Logger.
        /// </summary>
        /// <param name="logFile">File where logging is done.</param>
        public Logger(string logFile) {
            FileName = logFile;
        }

        /// <summary>
        /// File name for logging.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Log Content.
        /// </summary>
        /// <param name="content">Content to be logged.</param>
        /// <returns>Operation result detailing whether log was successful.</returns>
        public OperationResult Log(object content) =>
            Get.OperationResult(() => {
                if (string.IsNullOrWhiteSpace(FileName))
                    throw new ArgumentNullException("File Name is null. A file name must be provided before logging can begin.");
                lock(_firstLockToken) 
                    lock(_secondLockToken) 
                        using (var writer = File.AppendText(FileName))
                            writer.Write(content);
            });

    }
}