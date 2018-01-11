namespace CSharpToolkit.XAML {
    /// <summary>
    /// Denotes class who can raise CanExecuteChanged event.
    /// </summary>
    public interface IRaiseCanExecuteChanged {
        /// <summary>
        /// Raise CanExecuteChanged event.
        /// </summary>
        void RaiseCanExecuteChanged();
    }
}
