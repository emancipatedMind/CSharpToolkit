namespace CSharpToolkit.Utilities.EventArgs {
    /// <summary>
    /// A derived class of <see cref="System.EventArgs"/> that can represent a state change.
    /// </summary>
    /// <typeparam name="T">The type of the item whose state has changed.</typeparam>
    public class StateChangeEventArgs<T> : System.EventArgs {

        /// <summary>
        /// Instantiates the <see cref="StateChangeEventArgs{T}"/> class. 
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        public StateChangeEventArgs(T oldValue, T newValue) {
            OldValue = oldValue;
            NewValue = newValue;
        }
        /// <summary>
        /// The old value.
        /// </summary>
        public T OldValue { get; }
        /// <summary>
        /// The new value.
        /// </summary>
        public T NewValue { get; }
    }
}