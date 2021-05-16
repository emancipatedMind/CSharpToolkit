using CSharpToolkit.Logging.Abstractions;

namespace CSharpToolkit.Utilities.Abstractions {
    public interface IApplicationResources {
        IEmailLoggerAsync Emailer { get; }
        IExceptionLogger ErrorLogger { get; }
    }
}
