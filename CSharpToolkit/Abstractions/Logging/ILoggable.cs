namespace CSharpToolkit.Abstractions.Logging {
    /// <summary>
    /// Is adorned by class who may participate in logging.
    /// </summary>
    public interface ILoggable {
        /// <summary>
        /// Logger used for logging.
        /// </summary>
        ILogger Logger { set; }
    }
}
