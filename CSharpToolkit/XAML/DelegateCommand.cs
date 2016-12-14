using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CSharpToolkit.XAML {
    public class DelegateCommand : ICommand {
        protected readonly Action _command;
        protected Func<object, bool> _canExecuteMethod;

        public DelegateCommand(Action action, Func<object, bool> canExecuteLogic = null) {
            _command = action;
            _canExecuteMethod = canExecuteLogic;
        }

        public virtual void Execute(object parameter) {
            _command();
        }

        public bool CanExecute(object parameter) {
            return _canExecuteMethod?.Invoke(parameter) ?? true; 
        }

        #pragma warning disable 67
        public event EventHandler CanExecuteChanged;
        #pragma warning restore 67
    }
}
