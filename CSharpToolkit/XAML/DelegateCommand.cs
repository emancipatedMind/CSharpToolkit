namespace CSharpToolkit.XAML {
    using System;
    using System.Windows.Input;
    using Abstractions;
    /// <summary>
    /// Creates generic command that accepts object of type object as first parameter.
    /// </summary>
    public class DelegateCommand : DelegateCommand<object> {
        /// <summary>
        /// Creates generic command that accepts object of type object as first parameter.
        /// </summary>
        /// <param name="executeMethod">Method to run when command is invoked</param>
        public DelegateCommand(Action executeMethod) : base(_ => executeMethod(), null) { }

        /// <summary>
        /// Creates generic command that accepts object of type object as first parameter.
        /// </summary>
        /// <param name="executeMethod">Method to run when command is invoked.</param>
        /// <param name="canExecuteMethod">Method to determine if command can be run.</param>
        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod) : base(_ => executeMethod(), _ => canExecuteMethod()) { }
    }

    /// <summary>
    /// Creates generic command that accepts object of type T as first parameter.
    /// </summary>
    public class DelegateCommand<T> : ICommand, IRaiseCanExecuteChanged {
        private readonly Func<T, bool> _canExecuteMethod;
        private readonly Action<T> _executeMethod;
        private bool _isExecuting;

        /// <summary>
        /// Creates generic command that accepts object of type T as first parameter.
        /// </summary>
        /// <param name="executeMethod">Method to run when command is invoked</param>
        public DelegateCommand(Action<T> executeMethod) : this(executeMethod, null) { }

        /// <summary>
        /// Creates generic command that accepts object of type T as first parameter.
        /// </summary>
        /// <param name="executeMethod">Method to run when command is invoked.</param>
        /// <param name="canExecuteMethod">Method to determine if command can be run.</param>
        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod) {
            if ((executeMethod == null) && (canExecuteMethod == null)) {
                throw new ArgumentNullException("executeMethod", @"Execute Method cannot be null");
            }
            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
        }

        /// <summary>
        /// Raised anytime CanExecute has changed.
        /// </summary>
        public event EventHandler CanExecuteChanged {
            add {
                CommandManager.RequerySuggested += value;
            }
            remove {
                CommandManager.RequerySuggested -= value;
            }
        }

        /// <summary>
        /// Fire CanExecuteChanged event.
        /// </summary>
        public void RaiseCanExecuteChanged() {
            CommandManager.InvalidateRequerySuggested();
        }

        bool ICommand.CanExecute(object parameter) {
            return !_isExecuting && CanExecute((T)parameter);
        }

        void ICommand.Execute(object parameter) {
            _isExecuting = true;
            try {
                RaiseCanExecuteChanged();
                Execute((T)parameter);
            }
            finally {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Determines whether or not command can run.
        /// </summary>
        /// <param name="parameter">Command parameter.</param>
        /// <returns>Bool representing whether command can run(True) or not(False).</returns>
        public bool CanExecute(T parameter) {
            if (_canExecuteMethod == null)
                return true;

            return _canExecuteMethod(parameter);
        }

        /// <summary>
        /// Executes command.
        /// </summary>
        /// <param name="parameter">Command parameter.</param>
        public void Execute(T parameter) {
            _executeMethod(parameter);
        }

    }
}
