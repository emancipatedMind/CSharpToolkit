namespace CSharpToolkit.Logging.Abstractions {
    /// <summary>
    /// Implemented by logging class whose file name may be swapped.
    /// </summary>
    public interface IFileNameSwappableLogger : ILogger, IFileNameSwappable {
    }
}
