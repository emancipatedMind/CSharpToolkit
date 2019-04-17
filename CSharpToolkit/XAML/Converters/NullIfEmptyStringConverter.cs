namespace CSharpToolkit.XAML.Converters {
    using System;
    using System.Globalization;
    using System.Windows.Data;
    /// <summary>
    /// A converter used by comboboxes to unset the value.
    /// </summary>
    public class NullIfEmptyStringConverter : IValueConverter {

        /// <summary>
        /// Converts value to empty string if null. Otherwise, passes value through.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="targetType">(Unused.)</param>
        /// <param name="parameter">(Unused.)</param>
        /// <param name="culture">(Unused.)</param>
        /// <returns>If value is null, return 0. Other returns value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value == null ? "" : value;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType">(Unused.)</param>
        /// <param name="parameter">(Unused.)</param>
        /// <param name="culture">(Unused.)</param>
        /// <returns>If value is empty string, returns null. Otherwise, returns value.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            value?.Equals("") ?? false ? null : value;
    }
}