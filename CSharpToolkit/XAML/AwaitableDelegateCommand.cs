namespace CSharpToolkit.XAML {
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Utilities.EventArgs;
    using Abstractions;
    /// <summary>
    /// Creates awaitable generic command that accepts object of type object as first parameter.
    /// </summary>
    public class AwaitableDelegateCommand : AwaitableDelegateCommand<object>, IAsyncCommand {

        /// <summary>
        /// Creates awaitable generic command that accepts object of type object as first parameter.
        /// </summary>
        /// <param name="executeMethod">Method to run when command is invoked</param>
        public AwaitableDelegateCommand(Func<Task> executeMethod)
            : base(o => executeMethod()) { }

        /// <summary>
        /// Creates awaitable generic command that accepts object of type object as first parameter.
        /// </summary>
        /// <param name="executeMethod">Method to run when command is invoked.</param>
        /// <param name="canExecuteMethod">Method to determine if command can be run.</param>
        public AwaitableDelegateCommand(Func<Task> executeMethod, Func<bool> canExecuteMethod)
            : base(o => executeMethod(), o => canExecuteMethod()) { }
    }

    /// <summary>
    /// Creates awaitable generic command that accepts object of type T as first parameter.
    /// </summary>
    public class AwaitableDelegateCommand<T> : AwaitableDelegateCommandBase, IAsyncCommand<T>, ICommand {
        private readonly Func<T, Task> _executeMethod;
        private readonly DelegateCommand<T> _underlyingCommand;
        private bool _isExecuting;

        /// <summary>
        /// Creates awaitable generic command that accepts object of type T as first parameter.
        /// </summary>
        /// <param name="executeMethod">Method to run when command is invoked</param>
        public AwaitableDelegateCommand(Func<T, Task> executeMethod)
            : this(executeMethod, _ => true) { }

        /// <summary>
        /// Creates awaitable generic command that accepts object of type T as first parameter.
        /// </summary>
        /// <param name="executeMethod">Method to run when command is invoked</param>
        /// <param name="canExecuteMethod">Method to determine if command can be run.</param>
        public AwaitableDelegateCommand(Func<T, Task> executeMethod, Func<T, bool> canExecuteMethod) {
            _executeMethod = executeMethod;
            _underlyingCommand = new DelegateCommand<T>(x => { }, canExecuteMethod);
        }

        /// <summary>
        /// Executes command asynchronously.
        /// </summary>
        /// <param name="obj">Command parameter.</param>
        /// <returns>Object which represents asynchronous operation.</returns>
        public async Task ExecuteAsync(T obj) {
            try {
                try {
                    _isExecuting = true;
                    RaiseCanExecuteChanged();
                    await _executeMethod(obj);
                }
                finally {
                    _isExecuting = false;
                    RaiseCanExecuteChanged();
                }
            }
            catch (Exception ex) {
                bool handled = OnUnHandledException(this, ex);
                if (handled == false)
                    throw;
            }
        }

        /// <summary>
        /// This command.
        /// </summary>
        public ICommand Command => this;

        /// <summary>
        /// Determines whether or not command can run.
        /// </summary>
        /// <param name="parameter">Command parameter.</param>
        /// <returns>Bool representing whether command can run(True) or not(False).</returns>
        public bool CanExecute(object parameter) =>
            !_isExecuting && _underlyingCommand.CanExecute((T)parameter);

        /// <summary>
        /// Raised anytime CanExecute has changed.
        /// </summary>
        public event EventHandler CanExecuteChanged {
            add { _underlyingCommand.CanExecuteChanged += value; }
            remove { _underlyingCommand.CanExecuteChanged -= value; }
        }

        /// <summary>
        /// Executes command.
        /// </summary>
        /// <param name="parameter">Command parameter.</param>
        public async void Execute(object parameter) =>
            await ExecuteAsync((T)parameter);

        /// <summary>
        /// Fire CanExecuteChanged event.
        /// </summary>
        public void RaiseCanExecuteChanged() =>
            _underlyingCommand.RaiseCanExecuteChanged();
    }
}
