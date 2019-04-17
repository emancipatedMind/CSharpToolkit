namespace CSharpToolkit.Logging.Abstractions {
    using System;
    using System.Threading.Tasks;
    using Utilities;
    /// <summary>
    /// Implemented by class who may asynchronously log exceptions.
    /// </summary>
    public interface IExceptionLoggerAsync {
        /// <summary>
        /// Log Exception.
        /// </summary>
        /// <param name="exceptions">Exceptions to be logged.</param>
        /// <returns>Operation result detailing whether log was successful.</returns>
        Task<OperationResult> LogExceptionsAsync(params Exception[] exceptions);
    }
}