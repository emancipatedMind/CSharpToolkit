namespace CSharpToolkit.XAML.Behaviors {
    using System;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows;
    using System.Collections;
    using System.Windows.Data;
    using System.Windows.Input;
    public class UIElementBehaviors {



        public static ICommand GetCommandAfterLostFocus(DependencyObject obj) {
            return (ICommand)obj.GetValue(CommandAfterLostFocusProperty);
        }

        public static void SetCommandAfterLostFocus(DependencyObject obj, ICommand value) {
            obj.SetValue(CommandAfterLostFocusProperty, value);
        }

        // Using a DependencyProperty as the backing store for CommandAfterLostFocus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandAfterLostFocusProperty =
            DependencyProperty.RegisterAttached("CommandAfterLostFocus", typeof(ICommand), typeof(UIElementBehaviors), new PropertyMetadata(null, CommandAfterLostFocus_PropertyChanged));

        private static void CommandAfterLostFocus_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is UIElement == false)
                return;
            var element = d as UIElement;

            if (e.NewValue != null) {
                if (e.OldValue == null)
                    element.LostFocus += CommandAfterLostFocus_LostFocus;
            }
            else
                element.LostFocus -= CommandAfterLostFocus_LostFocus;
        }

        private static void CommandAfterLostFocus_LostFocus(object sender, RoutedEventArgs e) {
            var element = (UIElement)sender;
            GetCommandAfterLostFocus(element).Execute(GetCommandAfterLostFocusParameter(element) ?? new object());
        }

        public static object GetCommandAfterLostFocusParameter(DependencyObject obj) =>
            obj.GetValue(CommandAfterLostFocusParameterProperty);

        public static void SetCommandAfterLostFocusParameter(DependencyObject obj, object value) =>
            obj.SetValue(CommandAfterLostFocusParameterProperty, value);

        // Using a DependencyProperty as the backing store for CommandAfterLostFocusParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandAfterLostFocusParameterProperty =
            DependencyProperty.RegisterAttached("CommandAfterLostFocusParameter", typeof(object), typeof(UIElementBehaviors), new PropertyMetadata(null));

    }
}
