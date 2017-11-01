namespace CSharpToolkit.XAML {
    using System.Windows.Input;
    public static class CommandExtensions {
        public static void RaiseCanExecuteChanged(this ICommand command) {
            (command as IRaiseCanExecuteChanged)?.RaiseCanExecuteChanged();
        }
    }
}
