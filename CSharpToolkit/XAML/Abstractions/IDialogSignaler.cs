namespace CSharpToolkit.XAML.Abstractions {
    using System;
    /// <summary>
    /// Used by class to participate in dialog signaling.
    /// </summary>
    /// <typeparam name="T">Type of EventArgs when dialog is successful.</typeparam>
    public interface IDialogSignaler<T> where T : EventArgs {
        /// <summary>
        /// Raised when IDialogSignaler is cancelled.
        /// </summary>
        event EventHandler Cancelled;
        /// <summary>
        /// Raised when IDialogSignaler signals success.
        /// </summary>
        event EventHandler<T> Successful;
    }
}