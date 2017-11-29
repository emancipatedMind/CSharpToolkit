namespace CSharpToolkit.Logging {
    using System;
    using Abstractions.Logging;
    using EventArgs;
    public class ExceptionLogger : IExceptionLogger {

        IFileNameSwappableLogger _internalLogger;
        IExceptionFormatter _formatter;

        public ExceptionLogger(IFileNameSwappableLogger internalLogger, IExceptionFormatter formatter) {
            _formatter = formatter;
            _internalLogger = internalLogger;
            _internalLogger.LogOutputFailure += (s, e) => LogOutputFailure?.Invoke(this, e);
        } 

        public string FileName { set { _internalLogger.FileName = value; } }

        public bool LoggerFaulted => _internalLogger.LoggerFaulted;

        public event EventHandler<GenericEventArgs<Exception>> LogOutputFailure;

        public void Log(string content) => _internalLogger.Log(content);

        public void LogException(Exception ex) => Log(_formatter.FormatException(ex)); 

    }
}
