namespace CSharpToolkit.Validation {
    using System;
    using CSharpToolkit.Utilities;
    public class CallbackValidator<T> : Abstractions.IValidate<T> {
        private Func<T, OperationResult> _callback;

        public CallbackValidator(Func<T, OperationResult> callback) {
            _callback = callback;
        }

        public OperationResult Validate(T order) => _callback?.Invoke(order) ?? new OperationResult();
    }
}