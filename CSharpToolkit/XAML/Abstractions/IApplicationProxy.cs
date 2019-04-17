namespace CSharpToolkit.XAML.Abstractions {
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Threading;
    /// <summary>
    /// An interface that is proxy representation of a WPF application.
    /// </summary>
    public interface IApplicationProxy {
        /// <summary>
        /// The main window.
        /// </summary>
        Window MainWindow { get; set; }
        /// <summary>
        /// The windows of the application.
        /// </summary>
        IEnumerable<Window> Windows { get; }
        /// <summary>
        /// Shutdown the application.
        /// </summary>
        void Shutdown();

        /// <summary>
        /// A flag to denote that shut down has been called.
        /// </summary>
        bool HasBeenShutDown { get; }

        /// <summary>
        /// The dispatcher of the application. 
        /// </summary>
        Dispatcher Dispatcher { get; }
    }
}