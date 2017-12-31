namespace CSharpToolkit.Abstractions {
    /*
    What needs to happen?

    If the window is canceled, the viewmodel needs to know.
        'canceled' => Closed by X, or Quit/Cancel hit.

    If the viewmodel is signaled that it's successfully finished its operation, the window needs to close.  
        'successfully finished its operation' => Continue/Okay button hit.
    */

    public interface IDialog<T> where T : System.EventArgs  {
        System.Windows.Input.ICommand Cancel { get; }
        System.Windows.Input.ICommand Continue { get; }
        event System.EventHandler Canceled;
        event System.EventHandler<T> Successful;
    }
}
