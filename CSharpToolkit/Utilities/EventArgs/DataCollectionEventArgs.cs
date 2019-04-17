namespace CSharpToolkit.Utilities.EventArgs {
    using System;
    /// <summary>
    /// Used to collect data from event listeners.
    /// </summary>
    /// <typeparam name="T">Data Type.</typeparam>
    public class DataCollectionEventArgs<T> : EventArgs {
        /// <summary>
        /// Contains data collected.
        /// </summary>
        public T DataCollected { get; set; }
    }
}
