namespace CSharpToolkit.XAML.Behaviors {
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    public class ControlBehaviors {

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
                control.MouseDoubleClick += Control_MouseDoubleClick;
            } 
            else if (e.OldValue != null && e.NewValue == null) {
                control.MouseDoubleClick -= Control_MouseDoubleClick;
            } 
        }

        private static void Control_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (sender is Control == false)
                return;

            var control = (Control)sender;

            ICommand command = GetDoubleClickCommand(control);
            if (command == null)
                return;

            command.Execute(GetDoubleClickCommandParameter(control));
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

    }
}