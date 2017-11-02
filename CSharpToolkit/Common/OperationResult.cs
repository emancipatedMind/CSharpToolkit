namespace CSharpToolkit.Common {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    public class OperationResult<T> {

        public OperationResult(T result) : this(true, result) { }
        public OperationResult(IEnumerable<Exception> exceptions) : this(false, default(T), exceptions) { }
        public OperationResult(bool wasSuccessful, T result) : this(wasSuccessful, result, null) { }
        public OperationResult(bool wasSuccessful, T result, IEnumerable<Exception> exceptions) {
            Result = result;
            WasSuccessful = wasSuccessful;
            Exceptions = exceptions?.ToArray() ?? new Exception[0];
        }

        public bool HadErrors => Exceptions.Any();
        public bool WasSuccessful { get; }
        public Exception[] Exceptions { get; }
        public T Result { get; }
    }

    public class OperationResult {

        public OperationResult() : this (true, new Exception[0]) { }
        public OperationResult(bool wasSuccesful) : this (wasSuccesful, new Exception[0]) { }
        public OperationResult(IEnumerable<Exception> exceptions) : this(false, exceptions)  { }
        public OperationResult(OperationResult result) : this(result.WasSuccessful, result.Exceptions) { }
        public OperationResult(bool wasSuccessful, IEnumerable<Exception> exceptions) {
            WasSuccessful = wasSuccessful;
            Exceptions = exceptions?.ToArray() ?? new Exception[0];
        }

        public bool WasSuccessful { get; }
        public bool HadErrors => Exceptions.Any();
        public Exception[] Exceptions { get; }

    }
}