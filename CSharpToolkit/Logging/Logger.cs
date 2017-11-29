namespace CSharpToolkit.Logging {
    using System;
    using System.IO;
    using Abstractions.Logging;
    using EventArgs;
    public class Logger : IFileNameSwappableLogger {

        string _fileName;

        public Logger() : this("") { }
        public Logger(string logFile) {
            _fileName = logFile;
        }

        public string FileName { set { _fileName = value; } }

        public event EventHandler<GenericEventArgs<Exception>> LogOutputFailure;

        public bool LoggerFaulted { get; private set; } = false;

        public void Log(string content) {
            if (LoggerFaulted) return;
            try {
                using (var writer = File.AppendText(_fileName))
                    writer.Write(content);
            }
            catch(Exception ex) {
                LoggerFaulted = true;
                LogOutputFailure?.Invoke(this, new GenericEventArgs<Exception>(ex));
            }
        }

    }
}
