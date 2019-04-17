namespace CSharpToolkit.Logging.Abstractions {
    using Utilities;
    /// <summary>
    /// Implemented by a class who may be used for simple logging.
    /// </summary>
    public interface ILogger {
        /// <summary>
        /// Log Content.
        /// </summary>
        /// <param name="content">Content to be logged.</param>
        /// <returns>Operation result detailing whether log was successful.</returns>
        OperationResult Log(object content);
    }
}