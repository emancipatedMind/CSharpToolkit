namespace CSharpToolkit.XAML.Behaviors {
    using Abstractions;
    using Extensions;
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    /// <summary>
    /// A behavior used by windows to participate in dialog operations. DataContext of window must implement
    /// CSharpToolkit.Abstractions.IDialogControl in order to use this behavior.
    /// </summary>
    public class CloseBehavior {

        /// <summary>
        /// YieldDialogControl's setter method.
        /// </summary>
        /// <param name="obj">Object for which property is requested.</param>
        /// <param name="value">New value for YieldDialogControl.</param>
        public static void SetYieldDialogControl(Window obj, bool value) =>
            obj.SetValue(YieldDialogControlProperty, value);

        /// <summary>
        /// Dependency property for YieldDialogControl.
        /// </summary>
        public static readonly DependencyProperty YieldDialogControlProperty =
            DependencyProperty.RegisterAttached(
                "YieldDialogControl",
                typeof(bool),
                typeof(CloseBehavior),
                new PropertyMetadata(false, OnPropertyChanged)
                );

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            bool argumentsAreInvalid =
                (d is Window && e.NewValue is bool) == false;

            if (argumentsAreInvalid)
                return;

            var window = (Window)d;
            var newValue = (bool)e.NewValue;

            var environment = GetEnvironment(window);

            if (newValue) {
                if (environment == null) {
                    window.DataContextChanged += Window_DataContextChanged;
                    window.Closed += RemoveEvents;
                    Window_DataContextChanged(window, new DependencyPropertyChangedEventArgs(FrameworkElement.DataContextProperty, null, window.DataContext));
                }
            }
            else {
                window.DataContextChanged -= Window_DataContextChanged;
                window.Closed -= RemoveEvents;
                (environment as IDisposable)?.Dispose();
                SetEnvironment(window, null);
            }

        }

        private static void RemoveEvents(object sender, EventArgs e) {
            if (sender is Window == false) return;
            var window = (Window)sender;

            window.DataContextChanged -= Window_DataContextChanged;
            window.Closed -= RemoveEvents;
        }

        private static void Window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
            Type dataContextType = e.NewValue?.GetType();
            if (sender is Window == false || dataContextType == null) return;
            var window = (Window)sender;

            (GetEnvironment(window) as IDisposable)?.Dispose();

            Type dialogControlInterface =
                dataContextType
                    .GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDialogSignaler<>));

            if (dialogControlInterface == null) return;

            Type environmentType = typeof(DialogSignalerEnvironment<>)
                .MakeGenericType(dialogControlInterface.GenericTypeArguments);

            // Begin Critical Section.
            // Defining this section in loops or anything like that seems to result in errors due to references not passed properly.
            EventInfo successfulEvent = environmentType.GetEvent(nameof(DialogSignalerEnvironment<EventArgs>.Successful));
            EventInfo cancelEvent = environmentType.GetEvent(nameof(DialogSignalerEnvironment<EventArgs>.Cancelled));
            EventInfo dialogClosedEvent = environmentType.GetEvent(nameof(DialogSignalerEnvironment<EventArgs>.DialogClosed));

            Delegate successfulDelegate =
                Delegate.CreateDelegate(successfulEvent.EventHandlerType, null, new EventHandler(CleanupWindow).Method);
            Delegate cancelDelegate =
                Delegate.CreateDelegate(cancelEvent.EventHandlerType, null, new EventHandler(CleanupWindow).Method);
            Delegate dialogClosedDelegate =
                Delegate.CreateDelegate(dialogClosedEvent.EventHandlerType, null, new EventHandler(DialogClosed_Closed).Method);

            object environment = Activator.CreateInstance(environmentType, new object[] { e.NewValue, sender });
            successfulEvent.AddEventHandler(environment, successfulDelegate);
            cancelEvent.AddEventHandler(environment, cancelDelegate);
            dialogClosedEvent.AddEventHandler(environment, dialogClosedDelegate);
            // End Critical Section.

            SetEnvironment((Window)sender, environment);
        }

        private static void CleanupWindow(object sender, EventArgs e) {
            PropertyInfo windowProperty = sender.GetType().GetProperty(nameof(DialogSignalerEnvironment<EventArgs>.Dialog));
            var window = (Window)windowProperty.GetValue(sender);
            if (sender is IDisposable)
                ((IDisposable)sender).Dispose();

            window.Close();
        }

        private static void DialogClosed_Closed(object sender, EventArgs e) {
            Type senderType = sender.GetType();

            PropertyInfo dialogControllerProperty = senderType.GetProperty(nameof(DialogSignalerEnvironment<EventArgs>.DialogController));
            object controller = dialogControllerProperty.GetValue(sender);

            PropertyInfo cancelCommandProperty = controller.GetType().GetProperty(nameof(IDialogControl<EventArgs>.Cancel));
            var cancelCommand = cancelCommandProperty.GetValue(controller);

            MethodInfo executeMethod = typeof(System.Windows.Input.ICommand).GetMethod(nameof(System.Windows.Input.ICommand.Execute));
            if (sender is IDisposable)
                ((IDisposable)sender).Dispose();

            executeMethod.Invoke(cancelCommand, new object[] { new object() });
        }

        public static object GetEnvironment(DependencyObject obj) {
            return (object)obj.GetValue(EnvironmentProperty);
        }

        public static void SetEnvironment(DependencyObject obj, object value) {
            obj.SetValue(EnvironmentProperty, value);
        }

        // Using a DependencyProperty as the backing store for Environment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnvironmentProperty =
            DependencyProperty.RegisterAttached("Environment", typeof(object), typeof(CloseBehavior), new PropertyMetadata(null));

    }
}