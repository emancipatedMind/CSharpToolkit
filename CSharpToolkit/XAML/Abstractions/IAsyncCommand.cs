namespace CSharpToolkit.XAML.Abstractions {
    using System.Threading.Tasks;
    using System.Windows.Input;
    /// <summary>
    /// Denotes class who can fire command asynchronously.
    /// </summary>
    /// <typeparam name="T">Command Parameter Type.</typeparam>
    public interface IAsyncCommand<in T> : IRaiseCanExecuteChanged {
        /// <summary>
        /// Executes command asynchronously.
        /// </summary>
        /// <param name="obj">Command parameter.</param>
        /// <returns>Object which represents asynchronous operation.</returns>
        Task ExecuteAsync(T obj);
        /// <summary>
        /// Determines whether or not command can run.
        /// </summary>
        /// <param name="obj">Command parameter.</param>
        /// <returns>Bool representing whether command can run(True) or not(False).</returns>
        bool CanExecute(object obj);
        /// <summary>
        /// This command.
        /// </summary>
        ICommand Command { get; }
    }

    /// <summary>
    /// Denotes class who can fire command asynchronously. Command type parameter is of Type object.
    /// </summary>
    public interface IAsyncCommand : IAsyncCommand<object> {
    }

}
