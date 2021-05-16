namespace CSharpToolkit.Extensions {
    using Utilities;
    using Utilities.Abstractions;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    public static class WindowFunctions {

        public static void BasicWindowSetup(this Window window) {
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            RemoveAllEvents(window);

            XAML.Behaviors.CloseBehavior.SetYieldDialogControl(window, true);
            XAML.Behaviors.FrameworkElementBehaviors.SetKeyboardFocusFirstItem(window, true);
            XAML.Behaviors.FrameworkElementBehaviors.SetYieldFocusControl(window, true);
            window.Closing += DisallowWindowCloseIfCritical;
            window.Closed += Window_Closed;
        }

        public static void RemoveAllEvents(this Window window) {
            window.Closed -= Window_Closed;
            window.Closing -= DisallowWindowCloseIfCritical;
        }

        public static bool ShouldExitBeCanceled(this FrameworkElement element) =>
            element.DataContext == null ? false : CheckObjectForCloseCancelCondition(element.DataContext);

        public static bool CheckObjectForCloseCancelCondition(this object obj) {
            if (obj == null)
                return false;

            if (StateIsInvalidForClose(obj as IViewModelStateProvider)) {
                return true;
            }

            object[] propertyValues =
                obj.GetType().GetProperties()
                .Where(InvalidPropertyFilter)
                .Select(prop => prop.GetValue(obj))
                .Where(item => item != null)
                .ToArray();

            if (propertyValues.Any() == false)
                return false;

            IViewModelStateProvider[] providers =  propertyValues
                .Where(item => item is IViewModelStateProvider)
                .Select(item => (IViewModelStateProvider)item)
                .ToArray();

            if (providers.Any() == false)
                return false;

            if (providers.Any(StateIsInvalidForClose))
                return true;

            if (propertyValues.Any(CheckObjectForCloseCancelCondition))
                return true;

            return false;
        }

        public static bool StateIsInvalidForClose(this IViewModelStateProvider provider) =>
            provider?.State == ViewModelState.CriticalOperation && provider.GetType().GetCustomAttribute(typeof(DoNotAllowCloseWhenCriticalAttribute)) != null;

        public static bool InvalidPropertyFilter(this PropertyInfo prop) => (
            prop.PropertyType.IsSubclassOf(typeof(System.ValueType))
            || prop.GetIndexParameters().Any()
        ) == false;

        static Window[] GetCollectionOfWindowAndChildWindows(this Window win) {
            return
            win == null ?
                new Window[0] :
                new[] { win }
                    .Concat(
                        win.OwnedWindows.Cast<Window>()
                            .SelectMany(GetCollectionOfWindowAndChildWindows)
                    ).ToArray();
        }

        static void DisallowWindowCloseIfCritical(object sender, CancelEventArgs e) {
            if (sender is Window == false)
                return;

            Window window = (Window)sender;

            bool shouldExitBeCanceled =
                GetCollectionOfWindowAndChildWindows(window)
                .Any(ShouldExitBeCanceled);

            if (shouldExitBeCanceled) {
                e.Cancel = true;
                MessageBox.Show("You cannot close a window in this state.\r\n\r\nPlease complete operation.");
                return;
            }
        }

        static void Window_Closed(object sender, System.EventArgs e) {
            if (sender is Window == false)
                return;

            RemoveAllEvents((Window)sender);
        }

    }
}