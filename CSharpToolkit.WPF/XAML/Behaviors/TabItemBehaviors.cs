namespace CSharpToolkit.XAML.Behaviors {
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    public class TabItemBehaviors {
        // -------------------- BEGIN - Single Click Command Behavior -------------------- 

        public static object GetSingleClickCommandParameter(DependencyObject obj) =>
            obj.GetValue(SingleClickCommandParameterProperty);

        public static void SetSingleClickCommandParameter(DependencyObject obj, object value) =>
            obj.SetValue(SingleClickCommandParameterProperty, value);

        // Using a DependencyProperty as the backing store for SingleClickCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SingleClickCommandParameterProperty =
            DependencyProperty.RegisterAttached("SingleClickCommandParameter", typeof(object), typeof(TabItemBehaviors), new PropertyMetadata(null));

        public static ICommand GetSingleClickCommand(DependencyObject obj) =>
            (ICommand)obj.GetValue(SingleClickCommandProperty);

        public static void SetSingleClickCommand(DependencyObject obj, ICommand value) =>
            obj.SetValue(SingleClickCommandProperty, value);

        // Using a DependencyProperty as the backing store for SingleClickCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SingleClickCommandProperty =
            DependencyProperty.RegisterAttached("SingleClickCommand", typeof(ICommand), typeof(TabItemBehaviors), new PropertyMetadata(null, SingleClickCommand_PropertyChanged));

        private static void SingleClickCommand_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is TabItem == false)
                return;

            var control = (TabItem)d;

            if (e.OldValue == null && e.NewValue != null) {
                control.MouseLeftButtonUp += TabItem_MouseSingleClick;
            }
            else if (e.OldValue != null && e.NewValue == null) {
                control.MouseLeftButtonUp -= TabItem_MouseSingleClick;
            }
        }

        private static void TabItem_MouseSingleClick(object sender, MouseButtonEventArgs e) {
            if (sender is TabItem == false || e.ClickCount != 1)
                return;

            var control = (TabItem)sender;

            ICommand command = GetSingleClickCommand(control);
            if (command == null)
                return;

            command.Execute(GetSingleClickCommandParameter(control) ?? new object());
        }

        // -------------------- END - Single Click Command Behavior -------------------- 

        // -------------------- BEGIN - Double Click Command Behavior -------------------- 

        public static object GetDoubleClickCommandParameter(DependencyObject obj) =>
            obj.GetValue(DoubleClickCommandParameterProperty);

        public static void SetDoubleClickCommandParameter(DependencyObject obj, object value) =>
            obj.SetValue(DoubleClickCommandParameterProperty, value);

        // Using a DependencyProperty as the backing store for DoubleClickCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DoubleClickCommandParameterProperty =
            DependencyProperty.RegisterAttached("DoubleClickCommandParameter", typeof(object), typeof(TabItemBehaviors), new PropertyMetadata(null));

        public static ICommand GetDoubleClickCommand(DependencyObject obj) =>
            (ICommand)obj.GetValue(DoubleClickCommandProperty);

        public static void SetDoubleClickCommand(DependencyObject obj, ICommand value) =>
            obj.SetValue(DoubleClickCommandProperty, value);

        // Using a DependencyProperty as the backing store for DoubleClickCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DoubleClickCommandProperty =
            DependencyProperty.RegisterAttached("DoubleClickCommand", typeof(ICommand), typeof(TabItemBehaviors), new PropertyMetadata(null, DoubleClickCommand_PropertyChanged));

        private static void DoubleClickCommand_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is TabItem == false)
                return;

            var control = (TabItem)d;

            if (e.OldValue == null && e.NewValue != null) {
                control.MouseLeftButtonUp += TabItem_MouseDoubleClick;
            }
            else if (e.OldValue != null && e.NewValue == null) {
                control.MouseLeftButtonUp -= TabItem_MouseDoubleClick;
            }
        }

        private static void TabItem_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (sender is TabItem == false || e.ClickCount != 2)
                return;

            var control = (TabItem)sender;

            ICommand command = GetDoubleClickCommand(control);
            if (command == null)
                return;

            command.Execute(GetDoubleClickCommandParameter(control) ?? new object());
        }

        // -------------------- END - Double Click Command Behavior -------------------- 
    }
}
