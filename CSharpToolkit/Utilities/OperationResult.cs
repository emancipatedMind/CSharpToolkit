namespace CSharpToolkit.Utilities {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    /// <summary>
    /// Class used to contain an operation. Can denote whether operation was successful and provide, or if faulted, contains exceptions thrown by operation.
    /// </summary>
    /// <typeparam name="T">Operation result type.</typeparam>
    public class OperationResult<T> {

        /// <summary>
        /// Instantiates OperationResult class. This overload of constructor assumes operaton was successful.
        /// </summary>
        /// <param name="result">Operaton result.</param>
        public OperationResult(T result) : this(true, result) { }
        /// <summary>
        /// Instantiates OperationResult class. This overload of constructor assumes operaton was faulted.
        /// </summary>
        /// <param name="exceptions">Exceptions thrown by operation.</param>
        public OperationResult(IEnumerable<Exception> exceptions) : this(false, default(T), exceptions) { }
        /// <summary>
        /// Instantiates OperationResult class. This overload of constructor allows operation success to be set along with resulting data.
        /// </summary>
        /// <param name="wasSuccessful">Whether operation was successful or not.</param>
        /// <param name="result">Operation result.</param>
        public OperationResult(bool wasSuccessful, T result) : this(wasSuccessful, result, null) { }
        /// <summary>
        /// Instantiates OperationResult class. This overload of constructor allows complete configuration of OperationResult.
        /// </summary>
        /// <param name="wasSuccessful">Whether operation was successful or not.</param>
        /// <param name="result">Operation result.</param>
        /// <param name="exceptions">Exceptions thrown by operation.</param>
        public OperationResult(bool wasSuccessful, T result, IEnumerable<Exception> exceptions) {
            Result = result;
            WasSuccessful = wasSuccessful;
            Exceptions = exceptions?.ToArray() ?? new Exception[0];
        }

        /// <summary>
        /// Denotes whether Exceptions was captured by OperationResult.
        /// </summary>
        public bool HadErrors => Exceptions.Any();
        /// <summary>
        /// Denotes whether OperationResult was deemed to be a success.
        /// </summary>
        public bool WasSuccessful { get; }
        /// <summary>
        /// Exceptions captured.
        /// </summary>
        public Exception[] Exceptions { get; }
        /// <summary>
        /// Operation Result.
        /// </summary>
        public T Result { get; }
    }

    /// <summary>
    /// Class used to contain an operation. Can denote whether operation was successful and provide, or if faulted, contains exceptions thrown by operation.
    /// </summary>
    public class OperationResult {
        /// <summary>
        /// Instantiates OperationResult class. This overload of constructor assumes operaton was successful.
        /// </summary>
        public OperationResult() : this (true, new Exception[0]) { }
        /// <summary>
        /// Instantiates OperationResult class. This overload of constructor allows operation success to be set.
        /// </summary>
        /// <param name="wasSuccessful">Whether operation was successful or not.</param>
        public OperationResult(bool wasSuccessful) : this (wasSuccessful, new Exception[0]) { }
        /// <summary>
        /// Instantiates OperationResult class. This overload of constructor assumes operaton was faulted.
        /// </summary>
        /// <param name="exceptions">Exceptions thrown by operation.</param>
        public OperationResult(IEnumerable<Exception> exceptions) : this(false, exceptions)  { }
        /// <summary>
        /// Instantiates OperationResult class. This overload creates clone of OperationResult.
        /// </summary>
        /// <param name="result">Operation result.</param>
        public OperationResult(OperationResult result) : this(result.WasSuccessful, result.Exceptions) { }
        /// <summary>
        /// Instantiates OperationResult class. This overload of constructor allows complete configuration of OperationResult.
        /// </summary>
        /// <param name="wasSuccessful">Whether operation was successful or not.</param>
        /// <param name="exceptions">Exceptions thrown by operation.</param>
        public OperationResult(bool wasSuccessful, IEnumerable<Exception> exceptions) {
            WasSuccessful = wasSuccessful;
            Exceptions = exceptions?.ToArray() ?? new Exception[0];
        }

        /// <summary>
        /// Denotes whether OperationResult was deemed to be a success.
        /// </summary>
        public bool WasSuccessful { get; }
        /// <summary>
        /// Denotes whether Exceptions was captured by OperationResult.
        /// </summary>
        public bool HadErrors => Exceptions.Any();
        /// <summary>
        /// Exceptions captured.
        /// </summary>
        public Exception[] Exceptions { get; }

    }
}