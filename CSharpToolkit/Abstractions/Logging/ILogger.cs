namespace CSharpToolkit.Abstractions.Logging {
    using System;
    using EventArgs;
    public interface ILogger {
        void Log(string content);
        event EventHandler<GenericEventArgs<Exception>> LogOutputFailure;
        bool LoggerFaulted { get; }
    }
}
