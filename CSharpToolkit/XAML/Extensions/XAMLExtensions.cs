namespace CSharpToolkit.XAML.Extensions {
    using System.Windows;
    /// <summary>
    /// Some extensions method for use by XAML.
    /// </summary>
    public static class XAMLExtensions {

        /// <summary>
        /// Used to procure the top level window.
        /// </summary>
        /// <param name="window">The window whose top window is requested.</param>
        /// <returns>The top window. If null is passed, null will be returned.</returns>
        public static Window GetTopParentWindow(this Window window) =>
            window?.Owner == null ? window : window.Owner.GetTopParentWindow();

    }
}