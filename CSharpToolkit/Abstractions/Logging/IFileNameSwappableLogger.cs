namespace CSharpToolkit.Abstractions.Logging {
    /// <summary>
    /// Implemented by logging class whose file name may be swapped.
    /// </summary>
    public interface IFileNameSwappableLogger : ILogger, IFileNameSwappable {
    }
}
