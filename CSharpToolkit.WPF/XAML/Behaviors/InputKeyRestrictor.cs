namespace CSharpToolkit.XAML.Behaviors {
    using System;
    using System.Windows;
    using System.Windows.Controls;
    /// <summary>
    /// A behavior used by UIElements to ensure only numeric keys are allowed.
    /// </summary>
    public class InputKeyRestrictor {

        /// <summary>
        /// Used to have UIElement opt-in to this behavior.
        /// </summary>
        /// <param name="obj">Object for which property is requested</param>
        /// <param name="value">Whether object will disallow non-numeric keys or not.</param>
        public static void SetPositiveIntegerKeysOnly(DependencyObject obj, bool value) => obj.SetValue(PositiveIntegerKeysOnlyProperty, value);

        /// <summary>
        /// Dependency property for PositiveIntegerKeysOnly.
        /// </summary>
        public static readonly DependencyProperty PositiveIntegerKeysOnlyProperty =
            DependencyProperty.RegisterAttached("PositiveIntegerKeysOnly", typeof(bool), typeof(InputKeyRestrictor), new PropertyMetadata(false, GetPropertyChangedCallback("[^0-9]")));

        /// <summary>
        /// Used to have UIElement opt-in to this behavior.
        /// </summary>
        /// <param name="obj">Object for which property is requested</param>
        /// <param name="value">Whether object will disallow non-numeric keys or not.</param>
        public static void SetIntegerKeysOnly(DependencyObject obj, bool value) => obj.SetValue(IntegerKeysOnlyProperty, value);

        /// <summary>
        /// Dependency property for IntegerKeysOnly.
        /// </summary>
        public static readonly DependencyProperty IntegerKeysOnlyProperty =
            DependencyProperty.RegisterAttached("IntegerKeysOnly", typeof(bool), typeof(InputKeyRestrictor), new PropertyMetadata(false, GetPropertyChangedCallback("[^-0-9]")));

        /// <summary>
        /// Used to have UIElement opt-in to this behavior.
        /// </summary>
        /// <param name="obj">Object for which property is requested</param>
        /// <param name="value">Whether object will disallow non-numeric keys save for the . or not.</param>
        public static void SetPositiveDecimalKeysOnly(DependencyObject obj, bool value) => obj.SetValue(PositiveDecimalKeysOnlyProperty, value);

        /// <summary>
        /// Dependency property for PositiveDecimalKeysOnly.
        /// </summary>
        public static readonly DependencyProperty PositiveDecimalKeysOnlyProperty =
            DependencyProperty.RegisterAttached("PositiveDecimalKeysOnly", typeof(bool), typeof(InputKeyRestrictor), new PropertyMetadata(false, GetPropertyChangedCallback(@"[^\.0-9]")));

        /// <summary>
        /// Used to have UIElement opt-in to this behavior.
        /// </summary>
        /// <param name="obj">Object for which property is requested</param>
        /// <param name="value">Whether object will disallow non-numeric keys save for the . or not.</param>
        public static void SetDecimalKeysOnly(DependencyObject obj, bool value) => obj.SetValue(DecimalKeysOnlyProperty, value);

        /// <summary>
        /// Dependency property for DecimalKeysOnly.
        /// </summary>
        public static readonly DependencyProperty DecimalKeysOnlyProperty =
            DependencyProperty.RegisterAttached("DecimalKeysOnly", typeof(bool), typeof(InputKeyRestrictor), new PropertyMetadata(false, GetPropertyChangedCallback(@"[^-\.0-9]")));

        /// <summary>
        /// Used to have UIElement opt-in to this behavior.
        /// </summary>
        /// <param name="obj">Object for which property is requested</param>
        /// <param name="value">Whether object will disallow non-numeric keys save for the /, and - or not.</param>
        public static void SetDateKeysOnly(DependencyObject obj, bool value) => obj.SetValue(DateKeysOnlyProperty, value);

        /// <summary>
        /// Dependency property for DateKeysOnly.
        /// </summary>
        public static readonly DependencyProperty DateKeysOnlyProperty =
            DependencyProperty.RegisterAttached("DateKeysOnly", typeof(bool), typeof(InputKeyRestrictor), new PropertyMetadata(false, GetPropertyChangedCallback(@"[^-/0-9]")));

        /// <summary>
        /// Used to have UIElement opt-in to this behavior.
        /// </summary>
        /// <param name="obj">Object for which property is requested</param>
        /// <param name="value">Whether object will disallow non-numeric keys save for the : or not.</param>
        public static void SetTimeKeysOnly(DependencyObject obj, bool value) => obj.SetValue(TimeKeysOnlyProperty, value);

        /// <summary>
        /// Dependency property for TimeKeysOnly.
        /// </summary>
        public static readonly DependencyProperty TimeKeysOnlyProperty =
            DependencyProperty.RegisterAttached("TimeKeysOnly", typeof(bool), typeof(InputKeyRestrictor), new PropertyMetadata(false, GetPropertyChangedCallback(@"[^0-9:]")));

        private static PropertyChangedCallback GetPropertyChangedCallback(string pattern) =>
            (d, e) => {
                if (d is UIElement == false)
                    return;

                var element = (UIElement)d;
                if ((bool)e.NewValue) {
                    element.PreviewTextInput += InputKeyRestrictor_PreviewTextInput;
                    SetKeyRestrictionPattern(element, pattern);
                    if (element is TextBox)
                        DataObject.AddPastingHandler(element, OnPaste);
                }
                else {
                    element.PreviewTextInput -= InputKeyRestrictor_PreviewTextInput;
                    SetKeyRestrictionPattern(element, null);
                    if (element is TextBox)
                        DataObject.RemovePastingHandler(element, OnPaste);
                }
            };

        private static void InputKeyRestrictor_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) {
            string pattern = GetKeyRestrictionPattern((DependencyObject)sender);
            e.Handled = System.Text.RegularExpressions.Regex.IsMatch(e.Text, pattern + "+");
        }

        private static void OnPaste(object sender, DataObjectPastingEventArgs e) {
            var textBox = (TextBox)sender;
            e.CancelCommand();

            bool notText = e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true) == false;
            if (notText)
                return;

            string text = e.SourceDataObject.GetData(DataFormats.UnicodeText, true) as string;
            string pattern = GetKeyRestrictionPattern(textBox);
            bool noInvalidCharacter = System.Text.RegularExpressions.Regex.IsMatch(text, pattern) == false;

            if (noInvalidCharacter)
                return;

            textBox.Text = System.Text.RegularExpressions.Regex.Replace(text, pattern, "");
        }

        private static string GetKeyRestrictionPattern(DependencyObject obj) => (string)obj.GetValue(KeyRestrictionPatternProperty);
        private static void SetKeyRestrictionPattern(DependencyObject obj, string value) => obj.SetValue(KeyRestrictionPatternProperty, value);
        // Using a DependencyProperty as the backing store for KeyRestrictionPattern.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty KeyRestrictionPatternProperty = DependencyProperty.RegisterAttached("KeyRestrictionPattern", typeof(string), typeof(InputKeyRestrictor), new PropertyMetadata(null));
    }
}