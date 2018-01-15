namespace CSharpToolkit.XAML.Behaviors {
    using Abstractions;
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    /// <summary>
    /// A behavior used by windows to participate in dialog operations. DataContext of window must implement
    /// CSharpToolkit.Abstractions.DialogControl in order to use this behavior.
    /// </summary>
    public class CloseBehavior {

        /// <summary>
        /// YieldDialogControl's getter method.
        /// </summary>
        /// <param name="obj">Object for which property is requested.</param>
        /// <returns>YieldDialogControl's status. Whether or not the window is participating in this behavior.</returns>
        public static bool GetYieldDialogControl(DependencyObject obj) =>
            (bool)obj.GetValue(YieldDialogControlProperty);

        /// <summary>
        /// YieldDialogControl's setter method.
        /// </summary>
        /// <param name="obj">Object for which property is requested.</param>
        /// <param name="value">New value for YieldDialogControl.</param>
        public static void SetYieldDialogControl(DependencyObject obj, bool value) =>
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
            if (ArgumentsAreInvalid(d, e)) return;

            var window = (Window)d;
            var newValue = (bool)e.NewValue;
            if (newValue) {
                window.DataContextChanged += Window_DataContextChanged;
                window.Closed += RemoveEvents;
            }
            else {
                window.DataContextChanged -= Window_DataContextChanged;
                window.Closed -= RemoveEvents;
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

            Type dialogControlInterface =
                dataContextType
                    .GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDialogSignaler<>));

            if (dialogControlInterface == null) return;

            Type environmentType = typeof(DialogSignalerEnvironment<>)
                .MakeGenericType(dialogControlInterface.GenericTypeArguments);

            EventInfo cancelEvent = environmentType.GetEvent(nameof(DialogSignalerEnvironment<EventArgs>.Cancelled));
            EventInfo dialogClosedEvent = environmentType.GetEvent(nameof(DialogSignalerEnvironment<EventArgs>.DialogClosed));
            EventInfo successfulEvent = environmentType.GetEvent(nameof(DialogSignalerEnvironment<EventArgs>.Successful));

            Delegate cancelDelegate =
                Delegate.CreateDelegate(cancelEvent.EventHandlerType, null, new EventHandler(CleanupWindow).Method);
            Delegate successfulDelegate =
                Delegate.CreateDelegate(successfulEvent.EventHandlerType, null, new EventHandler(CleanupWindow).Method);
            Delegate dialogClosedDelegate =
                Delegate.CreateDelegate(dialogClosedEvent.EventHandlerType, null, new EventHandler(DialogClosed_Closed).Method);

            object environment = Activator.CreateInstance(environmentType, new object[] { e.NewValue, sender });
            cancelEvent.AddEventHandler(environment, cancelDelegate);
            successfulEvent.AddEventHandler(environment, successfulDelegate);
            dialogClosedEvent.AddEventHandler(environment, dialogClosedDelegate);
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

            MethodInfo executeMethod = typeof(DelegateCommand).GetMethod(nameof(DelegateCommand.Execute));
            if (sender is IDisposable)
                ((IDisposable)sender).Dispose();

            executeMethod.Invoke(cancelCommand, new object[] { new object() });
        }

        private static bool ArgumentsAreInvalid(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            (d is Window && e.NewValue is bool) == false;

    }
}