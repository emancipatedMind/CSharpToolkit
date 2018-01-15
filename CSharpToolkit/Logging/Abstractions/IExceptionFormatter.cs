namespace CSharpToolkit.Logging.Abstractions {
    using System;
    /// <summary>
    /// Implemented by class who may format exceptions.
    /// </summary>
    public interface IExceptionFormatter {
        /// <summary>
        /// Format exception into string.
        /// </summary>
        /// <param name="ex">Exception to format.</param>
        /// <returns>Exception formatted as string.</returns>
        string FormatException(Exception ex); 
    }
}
