namespace CSharpToolkit.XAML.Behaviors {
    using Abstractions;
    using System;
    using System.Windows;
    using System.Windows.Input;

    public class FrameworkElementBehaviors {

        //** BEGIN - Have first item receive focus support
        public static bool GetKeyboardFocusFirstItem(DependencyObject obj) =>
            (bool)obj.GetValue(KeyboardFocusFirstItemOnLoadedProperty);

        public static void SetKeyboardFocusFirstItem(DependencyObject obj, bool value) =>
            obj.SetValue(KeyboardFocusFirstItemOnLoadedProperty, value);

        // Using a DependencyProperty as the backing store for KeyboardFocusFirstItemOnLoaded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeyboardFocusFirstItemOnLoadedProperty =
            DependencyProperty.RegisterAttached("KeyboardFocusFirstItemOnLoaded", typeof(bool), typeof(FrameworkElementBehaviors), new PropertyMetadata(false, KeyboardFocusFirstItemOnLoadedPropertyChanged));

        private static void KeyboardFocusFirstItemOnLoadedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is FrameworkElement == false)
                return;

            var sender = (FrameworkElement)d;
            if ((bool)e.NewValue) {
                sender.Loaded += FrameworkElement_Loaded;
            }
            else {
                sender.Loaded -= FrameworkElement_Loaded;
            }
        }

        private static void FrameworkElement_Loaded(object sender, RoutedEventArgs e) {
            if (sender is FrameworkElement == false)
                return;

            var element = (FrameworkElement)sender;
            element.MoveFocus(new System.Windows.Input.TraversalRequest(System.Windows.Input.FocusNavigationDirection.First));
            element.Loaded -= FrameworkElement_Loaded;
        }
        //** END - Have first item receive focus support

        //** BEGIN - Yield Focus Control
        public static bool GetYieldFocusControl(Window obj) =>
            (bool)obj.GetValue(YieldFocusControlProperty);

        public static void SetYieldFocusControl(Window obj, bool value) =>
            obj.SetValue(YieldFocusControlProperty, value);

        // Using a DependencyProperty as the backing store for YieldFocusControl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YieldFocusControlProperty =
            DependencyProperty.RegisterAttached("YieldFocusControl", typeof(bool), typeof(FrameworkElementBehaviors), new PropertyMetadata(false, YieldFocusControl_PropertyChanged));

        private static void YieldFocusControl_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            bool argumentsAreInvalid =
                (d is Window && e.NewValue is bool) == false;

            if (argumentsAreInvalid)
                return;

            var window = (Window)d;
            var newValue = (bool)e.NewValue;
            if (newValue) {
                window.DataContextChanged += Window_DataContextChanged;
                window.Closed += RemoveEvents;

                Window_DataContextChanged(window, new DependencyPropertyChangedEventArgs(FrameworkElement.DataContextProperty, null, window.DataContext));
            }
            else {
                window.DataContextChanged -= Window_DataContextChanged;
                window.Closed -= RemoveEvents;
            }
        }

        private static void Window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
            if (sender is Window == false)
                return;

            var window = (Window)sender;

            var environment = GetEnvironment(window);
            environment?.Dispose();
            SetEnvironment(window, null);

            IFocusChanger newChanger = e.NewValue as IFocusChanger;
            if (newChanger != null) {
                SetEnvironment(window, new Environment(window, newChanger));
            }

        }

        private static void RemoveEvents(object sender, EventArgs e) {
            if (sender is Window == false) return;
            var window = (Window)sender;

            window.DataContextChanged -= Window_DataContextChanged;
            window.Closed -= RemoveEvents;

            GetEnvironment(window)?.Dispose();
        }

        static Environment GetEnvironment(Window obj) =>
            (Environment)obj.GetValue(EnvironmentProperty);

        static void SetEnvironment(Window obj, Environment value) =>
            obj.SetValue(EnvironmentProperty, value);

        // Using a DependencyProperty as the backing store for Environment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnvironmentProperty =
            DependencyProperty.RegisterAttached("Environment", typeof(Environment), typeof(FrameworkElementBehaviors), new PropertyMetadata(null));

        class Environment : IDisposable {

            IFocusChanger _changer;
            Window _window;
            bool _disposed;

            public Environment(Window window, IFocusChanger changer) {
                _window = window;
                _changer = changer;

                _changer.FocusChangeRequested += Changer_FocusChangeRequested;
            }

            void Dispose(bool disposing) {
                if (!_disposed) {
                    if (disposing) {
                        _changer.FocusChangeRequested -= Changer_FocusChangeRequested;
                    } 
                    _disposed = true;
                }
            }

            public void Dispose() =>
                Dispose(true);

            private void Changer_FocusChangeRequested(object sender, Utilities.EventArgs.GenericEventArgs<System.Windows.Input.FocusNavigationDirection> e) =>
                (FocusManager.GetFocusedElement(_window) as FrameworkElement)?.MoveFocus(new TraversalRequest(e.Data));

        }
        //** END - Yield Focus Control

    }
}