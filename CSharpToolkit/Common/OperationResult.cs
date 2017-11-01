namespace CSharpToolkit.Common {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    public class OperationResult<T> {

        public OperationResult(IEnumerable<Exception> exceptions) : this(false, exceptions) { }
        public OperationResult(bool successful, IEnumerable<Exception> exceptions = null) {
            Successful = successful;
            Exceptions = exceptions?.ToArray() ?? new Exception[0];
        }

        public OperationResult(T result) : this(true, result) { }
        public OperationResult(bool successful, T result) : this(successful, result, null) { }
        public OperationResult(bool successful, T result, IEnumerable<Exception> exceptions) {
            Result = result;
            Successful = successful;
            Exceptions = exceptions?.ToArray() ?? new Exception[0];
        }

        public bool Successful { get; }
        public Exception[] Exceptions { get; }
        public T Result { get; }
    }

    public class OperationResult {

        public OperationResult(IEnumerable<Exception> exceptions) {
            Successful = false;
            Exceptions = exceptions?.ToArray() ?? new Exception[0];
        }

        public OperationResult(bool successful, IEnumerable<Exception> exceptions = null) {
            Successful = successful;
            Exceptions = exceptions?.ToArray() ?? new Exception[0];
        }

        public OperationResult() {
            Successful = true;
            Exceptions = new Exception[0];
        }

        public OperationResult(OperationResult result) {
            Successful = result.Successful;
            Exceptions = result.Exceptions;
        }

        public bool Successful { get; }
        public Exception[] Exceptions { get; }

    }
}
