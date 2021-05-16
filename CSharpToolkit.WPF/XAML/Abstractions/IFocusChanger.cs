namespace CSharpToolkit.XAML.Abstractions {
    public interface IFocusChanger {
        event System.EventHandler<Utilities.EventArgs.GenericEventArgs<System.Windows.Input.FocusNavigationDirection>> FocusChangeRequested;
    }
}
