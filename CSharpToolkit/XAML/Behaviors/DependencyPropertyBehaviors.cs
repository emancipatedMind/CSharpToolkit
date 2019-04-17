namespace CSharpToolkit.XAML.Behaviors {
    using System;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;
    public class DependencyPropertyBehaviors {

        public static ICommand GetSourceUpdatedCommand(DependencyObject obj) => (ICommand)obj.GetValue(SourceUpdatedCommandProperty);
        public static void SetSourceUpdatedCommand(DependencyObject obj, ICommand value) => obj.SetValue(SourceUpdatedCommandProperty, value);

        // Using a DependencyProperty as the backing store for SourceUpdatedCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceUpdatedCommandProperty =
            DependencyProperty.RegisterAttached("SourceUpdatedCommand", typeof(ICommand), typeof(DependencyPropertyBehaviors), new PropertyMetadata(null, SourceUpdatedCommand_PropertyChanged));

        private static void SourceUpdatedCommand_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (e.NewValue != null)
                if (e.OldValue == null) 
                    Binding.AddSourceUpdatedHandler(d, DependencyProperty_SourceUpdated);
            else
                Binding.RemoveSourceUpdatedHandler(d, DependencyProperty_SourceUpdated);
        }

        private static void DependencyProperty_SourceUpdated(object sender, DataTransferEventArgs e) {
            var element = (DependencyObject)sender;
            GetSourceUpdatedCommand(element).Execute(GetSourceUpdatedParameter(element) ?? new object());
        }

        public static ICommand GetTargetUpdatedCommand(DependencyObject obj) => (ICommand)obj.GetValue(TargetUpdatedCommandProperty);
        public static void SetTargetUpdatedCommand(DependencyObject obj, ICommand value) => obj.SetValue(TargetUpdatedCommandProperty, value);

        // Using a DependencyProperty as the backing store for TargetUpdatedCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetUpdatedCommandProperty =
            DependencyProperty.RegisterAttached("TargetUpdatedCommand", typeof(ICommand), typeof(DependencyPropertyBehaviors), new PropertyMetadata(null, TargetUpdatedCommand_PropertyChanged));

        private static void TargetUpdatedCommand_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (e.NewValue != null)
                if (e.OldValue == null) 
                    Binding.AddTargetUpdatedHandler(d, DependencyProperty_TargetUpdated);
            else
                Binding.RemoveTargetUpdatedHandler(d, DependencyProperty_TargetUpdated);
        }

        private static void DependencyProperty_TargetUpdated(object sender, DataTransferEventArgs e) {
            var element = (DependencyObject)sender;
            GetTargetUpdatedCommand(element).Execute(GetTargetUpdatedParameter(element) ?? new object());
        }

        public static object GetTargetUpdatedParameter(DependencyObject obj) => obj.GetValue(TargetUpdatedParameterProperty);
        public static void SetTargetUpdatedParameter(DependencyObject obj, object value) => obj.SetValue(TargetUpdatedParameterProperty, value);
        // Using a DependencyProperty as the backing store for TargetUpdatedParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetUpdatedParameterProperty =
            DependencyProperty.RegisterAttached("TargetUpdatedParameter", typeof(object), typeof(DependencyPropertyBehaviors), new PropertyMetadata(null));

        public static object GetSourceUpdatedParameter(DependencyObject obj) => obj.GetValue(SourceUpdatedParameterProperty);
        public static void SetSourceUpdatedParameter(DependencyObject obj, object value) => obj.SetValue(SourceUpdatedParameterProperty, value);
        // Using a DependencyProperty as the backing store for SourceUpdatedParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceUpdatedParameterProperty =
            DependencyProperty.RegisterAttached("SourceUpdatedParameter", typeof(object), typeof(DependencyPropertyBehaviors), new PropertyMetadata(null));

    }
}