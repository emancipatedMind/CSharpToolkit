﻿namespace CSharpToolkit.XAML {
    using Abstractions;
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    public class CloseBehavior {

        public static bool GetYieldDialogControl(DependencyObject obj) =>
            (bool)obj.GetValue(YieldDialogControlProperty);

        public static void SetYieldDialogControl(DependencyObject obj, bool value) =>
            obj.SetValue(YieldDialogControlProperty, value);

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

            EventInfo cancelEvent = environmentType.GetEvent("Cancelled");
            EventInfo dialogClosedEvent = environmentType.GetEvent("DialogClosed");
            EventInfo successfulEvent = environmentType.GetEvent("Successful");

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
            PropertyInfo windowProperty = sender.GetType().GetProperty("Dialog");
            var window = (Window)windowProperty.GetValue(sender);
            if (sender is IDisposable)
                ((IDisposable)sender).Dispose();

            window.Close();
        }

        private static void DialogClosed_Closed(object sender, EventArgs e) {
            Type senderType = sender.GetType();

            PropertyInfo dialogControllerProperty = senderType.GetProperty("DialogController");
            object controller = dialogControllerProperty.GetValue(sender);

            PropertyInfo cancelCommandProperty = controller.GetType().GetProperty("Cancel");
            var cancelCommand = cancelCommandProperty.GetValue(controller);

            MethodInfo executeMethod = typeof(DelegateCommand).GetMethod("Execute");
            if (sender is IDisposable)
                ((IDisposable)sender).Dispose();

            executeMethod.Invoke(cancelCommand, new object[] { new object() });
        }

        private static bool ArgumentsAreInvalid(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            (d is Window && e.NewValue is bool) == false;

    }
}