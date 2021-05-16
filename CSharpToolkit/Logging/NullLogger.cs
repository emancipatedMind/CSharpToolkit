namespace CSharpToolkit.Logging {
    using Utilities;
    /// <summary>
    /// An implementation of the <see cref="Abstractions.ILogger"/> interface that simply logs to Output window.
    /// </summary>
    public class NullLogger : Abstractions.ILogger {

        static NullLogger _instance;
        /// <summary>
        /// An implementation of the singleton pattern for the <see cref="NullLogger"/> class.
        /// </summary>
        public static NullLogger Instance => _instance ?? (_instance = new NullLogger()); 

        object _firstToken = new object();
        object _secondToken = new object();

        private NullLogger() { }

        /// <summary>
        /// Simply Logs to the Output window.
        /// </summary>
        /// <param name="content">The content to log.</param>
        /// <returns>Denotes whether operation was successful. Always returns successfully without error.</returns>
        public OperationResult Log(object content) {
            lock(_firstToken) 
                lock(_secondToken) 
                    System.Diagnostics.Debug.WriteLine($"{nameof(NullLogger)} : {content}");

            return new OperationResult();
        }
    }
}