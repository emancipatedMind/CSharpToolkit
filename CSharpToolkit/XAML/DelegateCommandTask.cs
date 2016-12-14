using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpToolkit.XAML {
    public class DelegateCommandTask : DelegateCommand {

        public DelegateCommandTask(Action action, Func<object, bool> canExecuteLogic = null) : base(action, canExecuteLogic) { }

        public override void Execute(object parameter) {
            Task.Run(
                () => _command()
            );
        }
    }
}
