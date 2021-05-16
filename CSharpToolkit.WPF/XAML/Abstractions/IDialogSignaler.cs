namespace CSharpToolkit.XAML.Abstractions {
    using System;
    /// <summary>
    /// Used by class to participate in dialog signaling. Use <see cref="IDialogControl{T}"/> when participating in dialog control. Internal use only.
    /// </summary>
    /// <typeparam name="TEventArgs">Type of EventArgs when dialog is successful.</typeparam>
    public interface IDialogSignaler<TEventArgs> where TEventArgs : EventArgs {
        /// <summary>
        /// Raised when IDialogSignaler is cancelled.
        /// </summary>
        event EventHandler Cancelled;
        /// <summary>
        /// Raised when IDialogSignaler signals success.
        /// </summary>
        event EventHandler<TEventArgs> Successful;
    }
}