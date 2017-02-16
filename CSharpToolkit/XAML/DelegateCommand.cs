using System;
using System.Windows.Input;

namespace CSharpToolkit.XAML {
    public class DelegateCommand : ICommand {
        protected readonly Action<object> _command;
        protected Func<object, bool> _canExecuteMethod;

        public DelegateCommand(Action<object> action, Func<object, bool> canExecuteLogic = null) {
            _command = action;
            _canExecuteMethod = canExecuteLogic;
        }

        public virtual void Execute(object p) {
            _command(p);
        }

        public bool CanExecute(object parameter) {
            return _canExecuteMethod?.Invoke(parameter) ?? true;
        }

        #pragma warning disable 67
        public event EventHandler CanExecuteChanged {
            add {
                CommandManager.RequerySuggested += value;
            }
            remove {
                CommandManager.RequerySuggested -= value;
            }
        }
        #pragma warning restore 67
    }
}
