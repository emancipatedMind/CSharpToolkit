namespace CSharpToolkit.Abstractions.Logging {
    using System;
    public interface IExceptionLogger : IFileNameSwappableLogger {
        void LogException(Exception ex); 
    }
}
