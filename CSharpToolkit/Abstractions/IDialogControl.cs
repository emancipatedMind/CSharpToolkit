namespace CSharpToolkit.Abstractions {
    public interface IDialogControl<T> : IDialogSignaler<T> where T : System.EventArgs  {
        System.Windows.Input.ICommand Cancel { get; }
    }
}