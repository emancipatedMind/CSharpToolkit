namespace CSharpToolkit.Logging.Abstractions {
    using System;
    using Utilities;
    /// <summary>
    /// Implemented by class who may log exceptions.
    /// </summary>
    public interface IExceptionLogger {
        /// <summary>
        /// Log Exception.
        /// </summary>
        /// <param name="exceptions">Exceptions to be logged.</param>
        /// <returns>Operation result detailing whether log was successful.</returns>
        OperationResult LogExceptions(params Exception[] exceptions);
    }
}