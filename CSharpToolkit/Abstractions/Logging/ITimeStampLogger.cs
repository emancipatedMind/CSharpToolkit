namespace CSharpToolkit.Abstractions.Logging {
    using System;
    public interface ITimeStampLogger : ILogger {
        DateTime LogWithCurrentTime(string content, string dateSeparationString, bool prependNewLine);
    }
}
