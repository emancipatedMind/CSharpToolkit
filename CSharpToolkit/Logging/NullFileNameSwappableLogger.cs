namespace CSharpToolkit.Logging {
    using System;
    using Abstractions;
    using Utilities;
    /// <summary>
    /// An implementation of the <see cref="IFileNameSwappableLogger"/> interface that simply logs to Output window.
    /// </summary>
    public class NullFileNameSwappableLogger : IFileNameSwappableLogger {
        static NullFileNameSwappableLogger _instance;
        /// <summary>
        /// An implementation of the singleton pattern for the <see cref="NullFileNameSwappableLogger"/> class.
        /// </summary>
        public static NullFileNameSwappableLogger Instance => _instance ?? (_instance = new NullFileNameSwappableLogger());

        /// <summary>
        /// File name for logging.
        /// </summary>
        public string FileName { get; set; }

        object _firstToken = new object();
        object _secondToken = new object();

        NullFileNameSwappableLogger() { }

        /// <summary>
        /// Simply Logs to the Output window.
        /// </summary>
        /// <param name="content">The content to log.</param>
        /// <returns>Denotes whether operation was successful. Always returns successfully without error.</returns>
        public OperationResult Log(object content) {
            lock(_firstToken) 
                lock(_secondToken) 
                    System.Diagnostics.Debug.WriteLine($"{nameof(NullFileNameSwappableLogger)} : {content}");

            return new OperationResult();
        }
    }
}