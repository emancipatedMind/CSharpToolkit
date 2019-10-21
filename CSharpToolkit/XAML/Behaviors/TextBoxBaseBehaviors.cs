using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace CSharpToolkit.XAML.Behaviors {
    public class TextBoxBaseBehaviors {


        public static bool GetHighlightAllOnTabEntry(DependencyObject obj) {
            return (bool)obj.GetValue(HighlightAllOnTabEntryProperty);
        }

        public static void SetHighlightAllOnTabEntry(DependencyObject obj, bool value) {
            obj.SetValue(HighlightAllOnTabEntryProperty, value);
        }

        // Using a DependencyProperty as the backing store for HighlightAllOnTabEntry.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighlightAllOnTabEntryProperty =
            DependencyProperty.RegisterAttached("HighlightAllOnTabEntry", typeof(bool), typeof(TextBoxBaseBehaviors), new PropertyMetadata(false, HighlightAllOnTabEntry_PropertyChanged));

        private static void HighlightAllOnTabEntry_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is TextBoxBase == false)
                return;
            var textBox = (TextBoxBase)d; 

            if ((bool)e.NewValue) {
                textBox.GotKeyboardFocus += TextBox_GotKeyboardFocus;
            }
            else {
                textBox.GotKeyboardFocus -= TextBox_GotKeyboardFocus;
            }
        }

        private static void TextBox_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e) {
            if (sender is TextBoxBase == false)
                return;

            if (e.KeyboardDevice.IsKeyDown(System.Windows.Input.Key.Tab)) {
                ((TextBoxBase)sender).SelectAll();
            } 

        }

    }
}
