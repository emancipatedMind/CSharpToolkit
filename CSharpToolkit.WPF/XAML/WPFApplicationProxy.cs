namespace CSharpToolkit.XAML {
    using Abstractions;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Threading;

    public class WPFApplicationProxy : IWPFApplicationProxy {

        public WPFApplicationProxy() {
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        }

        public IEnumerable<Window> Windows =>
            Application.Current?.Windows?.Cast<Window>() ?? new Window[0];

        public void Shutdown() =>
            Application.Current?.Dispatcher?.Invoke(() => Application.Current?.Shutdown());

        public Window MainWindow {
            get { return Application.Current?.MainWindow; }
            set {
                if (Application.Current != null) 
                    Application.Current.MainWindow = value;
            }
        }

        public bool HasBeenShutDown => Application.Current == null;

        public Dispatcher Dispatcher => Application.Current?.Dispatcher;

    }
}
