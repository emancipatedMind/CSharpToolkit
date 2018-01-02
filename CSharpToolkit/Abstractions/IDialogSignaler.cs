namespace CSharpToolkit.Abstractions {
    using System;
    public interface IDialogSignaler<T> where T : EventArgs {
        event EventHandler Cancelled;
        event EventHandler<T> Successful;
    }
}