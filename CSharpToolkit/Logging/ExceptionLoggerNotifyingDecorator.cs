namespace CSharpToolkit.Logging {
    using System;
    using Abstractions;
    using Utilities;
    using Utilities.EventArgs;

    /// <summary>
    /// A decorator of the <see cref="IExceptionLogger"/> interface that can notify through an event that an error has been logged, and whether that log was successful.
    /// </summary>
    public class ExceptionLoggerNotifyingDecorator : IExceptionLogger {

        IExceptionLogger _component;

        /// <summary>
        /// Instantiates <see cref="ExceptionLoggerNotifyingDecorator"/>.
        /// </summary>
        /// <param name="component"></param>
        public ExceptionLoggerNotifyingDecorator(IExceptionLogger component) {
            _component = component;
        }

        /// <summary>
        /// Log Exception.
        /// </summary>
        /// <param name="exceptions">Exceptions to be logged.</param>
        /// <returns>Operation result detailing whether log was successful.</returns>
        public OperationResult LogExceptions(params Exception[] exceptions) {
            var operation =
                _component.LogExceptions(exceptions);

            ErrorLogged?.Invoke(this, new GenericEventArgs<bool>(operation.WasSuccessful));
            return operation;
        }

        /// <summary>
        /// An event notifying that an error log was attempted along with a success indicator.
        /// </summary>
        public event EventHandler<GenericEventArgs<bool>> ErrorLogged;
    }
}