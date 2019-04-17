namespace CSharpToolkit.Utilities.Abstractions {
    /// <summary>
    /// Adorned by a class who will notify once disposing has finished.
    /// </summary>
    public interface INotifyDisposable : System.IDisposable {
        /// <summary>
        /// Notification that disposing has finished.
        /// </summary>
        event System.EventHandler Disposed;
    }
}