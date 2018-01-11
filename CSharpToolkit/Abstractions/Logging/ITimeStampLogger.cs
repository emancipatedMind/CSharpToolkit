namespace CSharpToolkit.Abstractions.Logging {
    using System;
    /// <summary>
    /// Implemented by class who may log time stamps.
    /// </summary>
    public interface ITimeStampLogger : ILogger {
        /// <summary>
        /// Logs current time.
        /// </summary>
        /// <param name="content">Content to log.</param>
        /// <param name="dateSeparationString">Token to separate content, and date.</param>
        /// <param name="prependNewLine">Whether a new line is prepended or not.</param>
        /// <returns>Time which was logged.</returns>
        DateTime LogWithCurrentTime(string content, string dateSeparationString, bool prependNewLine);
    }
}
