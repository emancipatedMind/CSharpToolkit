namespace CSharpToolkit.XAML.Behaviors {
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    public class ControlBehaviors {

        // -------------------- BEGIN - Single Click Command Behavior -------------------- 

        public static object GetSingleClickCommandParameter(DependencyObject obj) =>
            obj.GetValue(SingleClickCommandParameterProperty);

        public static void SetSingleClickCommandParameter(DependencyObject obj, object value) =>
            obj.SetValue(SingleClickCommandParameterProperty, value);

        // Using a DependencyProperty as the backing store for SingleClickCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SingleClickCommandParameterProperty =
            DependencyProperty.RegisterAttached("SingleClickCommandParameter", typeof(object), typeof(ControlBehaviors), new PropertyMetadata(null));

        public static ICommand GetSingleClickCommand(DependencyObject obj) =>
            (ICommand)obj.GetValue(SingleClickCommandProperty);

        public static void SetSingleClickCommand(DependencyObject obj, ICommand value) =>
            obj.SetValue(SingleClickCommandProperty, value);

        // Using a DependencyProperty as the backing store for SingleClickCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SingleClickCommandProperty =
            DependencyProperty.RegisterAttached("SingleClickCommand", typeof(ICommand), typeof(ControlBehaviors), new PropertyMetadata(null, SingleClickCommand_PropertyChanged));

        private static void SingleClickCommand_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is Control == false)
                return;

            var control = (Control)d;

            if (e.OldValue == null && e.NewValue != null) {
                control.MouseLeftButtonDown += Control_MouseSingleClick;
            }
            else if (e.OldValue != null && e.NewValue == null) {
                control.MouseLeftButtonDown -= Control_MouseSingleClick;
            }
        }

        private static void Control_MouseSingleClick(object sender, MouseButtonEventArgs e) {
            if (sender is Control == false || e.ClickCount != 1)
                return;

            var control = (Control)sender;

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
            DependencyProperty.RegisterAttached("DoubleClickCommandParameter", typeof(object), typeof(ControlBehaviors), new PropertyMetadata(null));

        public static ICommand GetDoubleClickCommand(DependencyObject obj) =>
            (ICommand)obj.GetValue(DoubleClickCommandProperty);

        public static void SetDoubleClickCommand(DependencyObject obj, ICommand value) =>
            obj.SetValue(DoubleClickCommandProperty, value);

        // Using a DependencyProperty as the backing store for DoubleClickCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DoubleClickCommandProperty =
            DependencyProperty.RegisterAttached("DoubleClickCommand", typeof(ICommand), typeof(ControlBehaviors), new PropertyMetadata(null, DoubleClickCommand_PropertyChanged));

        private static void DoubleClickCommand_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is Control == false)
                return;

            var control = (Control)d;

            if (e.OldValue == null && e.NewValue != null) {
                control.MouseLeftButtonDown += Control_MouseDoubleClick;
            }
            else if (e.OldValue != null && e.NewValue == null) {
                control.MouseLeftButtonDown -= Control_MouseDoubleClick;
            }
        }

        private static void Control_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (sender is Control == false || e.ClickCount != 2)
                return;

            var control = (Control)sender;

            ICommand command = GetDoubleClickCommand(control);
            if (command == null)
                return;

            command.Execute(GetDoubleClickCommandParameter(control) ?? new object());
        }

        // -------------------- END - Double Click Command Behavior -------------------- 


        public static readonly DependencyProperty IsAccessKeyScopeProperty =
            DependencyProperty.RegisterAttached("IsAccessKeyScope", typeof(bool), typeof(ControlBehaviors), new PropertyMetadata(false, HandleIsAccessKeyScopePropertyChanged));

        public static void SetIsAccessKeyScope(DependencyObject obj, bool value) => obj.SetValue(IsAccessKeyScopeProperty, value);

        public static bool GetIsAccessKeyScope(DependencyObject obj) => (bool)obj.GetValue(IsAccessKeyScopeProperty);

        private static void HandleIsAccessKeyScopePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (e.NewValue.Equals(true)) {
                AccessKeyManager.AddAccessKeyPressedHandler(d, HandleScopedElementAccessKeyPressed);
            }
            else {
                AccessKeyManager.RemoveAccessKeyPressedHandler(d, HandleScopedElementAccessKeyPressed);
            }
        }

        private static void HandleScopedElementAccessKeyPressed(object sender, AccessKeyPressedEventArgs e) {
            if (!Keyboard.IsKeyDown(Key.LeftAlt) && !Keyboard.IsKeyDown(Key.RightAlt) && GetIsAccessKeyScope((DependencyObject)sender)) {
                e.Scope = sender;
                e.Handled = true;
            }
        }

        public static readonly DependencyProperty UpdatePropertySourceWhenEnterPressedProperty =
            DependencyProperty.RegisterAttached("UpdatePropertySourceWhenEnterPressed", typeof(DependencyProperty), typeof(ControlBehaviors), new PropertyMetadata(null, OnUpdatePropertySourceWhenEnterPressedPropertyChanged));

        public static void SetUpdatePropertySourceWhenEnterPressed(DependencyObject dp, DependencyProperty value) =>
            dp.SetValue(UpdatePropertySourceWhenEnterPressedProperty, value);

        public static DependencyProperty GetUpdatePropertySourceWhenEnterPressed(DependencyObject dp) =>
            (DependencyProperty)dp.GetValue(UpdatePropertySourceWhenEnterPressedProperty);

        private static void OnUpdatePropertySourceWhenEnterPressedPropertyChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e) {
            if (dp is UIElement == false)
                return;

            UIElement element = (UIElement)dp;

            if (e.OldValue == null && e.NewValue != null) {
                element.PreviewKeyDown += HandlePreviewKeyDown;
            }
            else if (e.OldValue != null && e.NewValue == null) {
                element.PreviewKeyDown -= HandlePreviewKeyDown;
            }
        }

        static void HandlePreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter)
                DoUpdateSource(e.Source as UIElement);
        }

        static void DoUpdateSource(UIElement source) {
            DependencyProperty property = GetUpdatePropertySourceWhenEnterPressed((DependencyObject)source);

            if (property == null)
                return;

            BindingExpression binding = BindingOperations.GetBindingExpression(source, property);
            binding?.UpdateSource();
            binding?.UpdateTarget();
        }

    }
}