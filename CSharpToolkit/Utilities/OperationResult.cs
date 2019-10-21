namespace CSharpToolkit.Utilities {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    /// <summary>
    /// Class used to contain an operation. Can denote whether operation was successful and provide, if faulted, exceptions thrown by operation.
    /// </summary>
    /// <typeparam name="T">Operation result type.</typeparam>
    [System.Diagnostics.DebuggerStepThrough]
    public class OperationResult<T> : OperationResult {
        /// <summary>
        /// Instantiates <see cref="OperationResult"/> class. This overload assumes operaton was successful.
        /// </summary>
        /// <param name="result">Operaton result.</param>
        public OperationResult(T result) : this(true, result) { }
        /// <summary>
        /// Instantiates <see cref="OperationResult"/> class. This overload assumes operaton was faulted.
        /// </summary>
        /// <param name="exceptions">Exceptions thrown by operation.</param>
        public OperationResult(IEnumerable<Exception> exceptions) : this(false, default(T), exceptions) { }
        /// <summary>
        /// Instantiates <see cref="OperationResult"/> class. This overload allows operation success to be set along with resulting data.
        /// </summary>
        /// <param name="wasSuccessful">Whether operation was successful or not.</param>
        /// <param name="result">Operation result.</param>
        public OperationResult(bool wasSuccessful, T result) : this(wasSuccessful, result, null) { }
        /// <summary>
        /// Instantiates <see cref="OperationResult"/> class. This overload allows complete configuration of <see cref="OperationResult"/>.
        /// </summary>
        /// <param name="wasSuccessful">Whether operation was successful or not.</param>
        /// <param name="result">Operation result.</param>
        /// <param name="exceptions">Exceptions thrown by operation.</param>
        public OperationResult(bool wasSuccessful, T result, IEnumerable<Exception> exceptions) : base(wasSuccessful, exceptions) {
            Result = result;
        }
        /// <summary>
        /// Instantiates <see cref="OperationResult"/> class. This overload passes on the exceptions if any exist, and assumes operation was faulted.
        /// </summary>
        /// <param name="result"></param>
        public OperationResult(OperationResult result) : this(false, default(T), result.Exceptions) {
            ImportData(result);
        }
        /// <summary>
        /// The <see cref="Result"/> of the <see cref="OperationResult"/>.
        /// </summary>
        public T Result { get; }
    }

    /// <summary>
    /// Class used to contain an operation. Can denote whether operation was successful and provide, if faulted, exceptions thrown by operation.
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    public class OperationResult {
        /// <summary>
        /// Instantiates <see cref="OperationResult"/> class. This overload of constructor assumes operation was successful.
        /// </summary>
        public OperationResult() : this (true, new Exception[0]) { }
        /// <summary>
        /// Instantiates <see cref="OperationResult"/> class. This overload of constructor allows operation success to be set.
        /// </summary>
        /// <param name="wasSuccessful">Whether operation was successful or not.</param>
        public OperationResult(bool wasSuccessful) : this (wasSuccessful, new Exception[0]) { }
        /// <summary>
        /// Instantiates <see cref="OperationResult"/> class. This overload of constructor assumes operaton was faulted.
        /// </summary>
        /// <param name="exceptions">Exceptions thrown by operation.</param>
        public OperationResult(IEnumerable<Exception> exceptions) : this(false, exceptions)  { }
        /// <summary>
        /// Instantiates <see cref="OperationResult"/> class. This overload of constructor allows complete configuration of <see cref="OperationResult"/>.
        /// </summary>
        /// <param name="wasSuccessful">Whether operation was successful or not.</param>
        /// <param name="exceptions">Exceptions thrown by operation.</param>
        public OperationResult(bool wasSuccessful, IEnumerable<Exception> exceptions) {
            WasSuccessful = wasSuccessful;
            Exceptions = exceptions?.ToArray() ?? new Exception[0];
        }
        /// <summary>
        /// Instantiates <see cref="OperationResult"/> class. This overload of constructor passes on any errors, and any data found. If no errors, then <see cref="WasSuccessful"/> is true.
        /// </summary>
        public OperationResult(OperationResult result) {
            if (result.HadErrors)
                Exceptions = result.Exceptions;
            else
                WasSuccessful = true;
            ImportData(result);
        }
        /// <summary>
        /// Denotes whether <see cref="OperationResult"/> was deemed to be a success.
        /// </summary>
        public bool WasSuccessful { get; protected set; }
        /// <summary>
        /// Denotes whether Exceptions was captured by <see cref="OperationResult"/>.
        /// </summary>
        public bool HadErrors => Exceptions.Any();
        /// <summary>
        /// Exceptions captured.
        /// </summary>
        public Exception[] Exceptions { get; protected set; }
        /// <summary>
        /// Denotes whether any data was captured by <see cref="OperationResult"/>.
        /// </summary>
        public bool HasData => Data.Any();
        /// <summary>
        /// Random data that can be passed back to caller.
        /// </summary>
        public Dictionary<string, object> Data { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Used to copy data from one <see cref="OperationResult"/> to another.
        /// </summary>
        /// <param name="result"><see cref="OperationResult"/> whose data will be copied.</param>
        public void ImportData(OperationResult result) =>
            ImportData(result?.Data);
        /// <summary>
        /// Used to import data from a <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="data"><see cref="Dictionary{TKey, TValue}"/> to import data from.</param>
        public void ImportData(Dictionary<string, object> data) {
            data = data ?? new Dictionary<string, object>();
            foreach (string key in data.Keys)
                Data[key] = data[key];
        }
    }
}