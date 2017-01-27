using System;
using System.Threading.Tasks;

namespace CSharpToolkit.XAML {
    public class DelegateCommandTask : DelegateCommand {

        public DelegateCommandTask(Action<object> action, Func<object, bool> canExecuteLogic = null) : base(action, canExecuteLogic) { }

        public override void Execute(object p) {
            Task.Run(
                () => _command(p)
            );
        }
    }
}