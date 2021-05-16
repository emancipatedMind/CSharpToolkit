namespace CSharpToolkit.XAML.Behaviors {
    using System;
    using System.Windows;
    using System.Windows.Controls;
    public class PasswordBoxBehavior {

        public static void SetBindPassword(DependencyObject obj, bool value) {
            obj.SetValue(BindPasswordProperty, value);
        }

        // Using a DependencyProperty as the backing store for BindPassword.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BindPasswordProperty =
            DependencyProperty.RegisterAttached("BindPassword", typeof(bool), typeof(PasswordBoxBehavior), new PropertyMetadata(false, BindPassword_PropertyChanged));

        private static void BindPassword_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is PasswordBox == false)
                return;

            PasswordBox passwordBox = (PasswordBox)d;

            bool newValue = (bool)e.NewValue;

            if (newValue)
                passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
            else
                passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;

        }

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e) {
            if (sender is PasswordBox == false)
                return;
            PasswordBox box = (PasswordBox)sender;
            if (box.DataContext is Abstractions.IBindablePassword == false)
                return;

            ((Abstractions.IBindablePassword)box.DataContext).Password = box.SecurePassword;
        }

    }
}