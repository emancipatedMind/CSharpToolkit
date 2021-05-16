namespace CSharpToolkit.XAML.Abstractions {
    /// <summary>
    /// Used to give a class dialog control.
    /// </summary>
    /// <typeparam name="T">Type of EventArgs when dialog is successful.</typeparam>
    public interface IDialogControl<T> : IDialogSignaler<T> where T : System.EventArgs  {
        /// <summary>
        /// Cancel Command.
        /// </summary>
        System.Windows.Input.ICommand Cancel { get; }
    }
}