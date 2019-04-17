namespace CSharpToolkit.Utilities.Abstractions {
    /// <summary>
    /// Adorned by a class who may report a state change.
    /// </summary>
    public interface IStateChanged<T> {
        /// <summary>
        /// An event fired when the state has changed from one value to another.
        /// </summary>
        event System.EventHandler<EventArgs.StateChangeEventArgs<T>> StateChanged;
    }
}