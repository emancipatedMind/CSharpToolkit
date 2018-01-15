namespace CSharpToolkit.Logging.Abstractions {
    using System;
    using EventArgs;
    /// <summary>
    /// Implemented by a class who may be used for simple logging.
    /// </summary>
    public interface ILogger {
        /// <summary>
        /// Log Content.
        /// </summary>
        /// <param name="content">Content to be logged.</param>
        void Log(string content);
        /// <summary>
        /// Raied to indicate logger has failed to output to destination.
        /// </summary>
        event EventHandler<GenericEventArgs<Exception>> LogOutputFailure;
        /// <summary>
        /// Indicates whether Logger has faulted.
        /// </summary>
        bool LoggerFaulted { get; }
    }
}
