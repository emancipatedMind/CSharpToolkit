namespace CSharpToolkit.Extensions {
    using System.Windows.Input;
    using XAML.Abstractions;
    /// <summary>
    /// Extensions for use by objects of type ICommand.
    /// </summary>
    public static class CommandExtensions {
        /// <summary>
        /// Raise CanExecuteChanged if they also implement CSharpToolkit.XAML.IRaiseCanExecuteChanged.
        /// </summary>
        /// <param name="command">Source object.</param>
        public static void RaiseCanExecuteChanged(this ICommand command) {
            (command as IRaiseCanExecuteChanged)?.RaiseCanExecuteChanged();
        }
    }
}
