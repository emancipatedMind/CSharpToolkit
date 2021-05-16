namespace CSharpToolkit.XAML.Converters {
    using System;
    using System.Globalization;
    using System.Windows.Data;
    /// <summary>
    /// Used to verify that value can be represented as decimal or float.
    /// </summary>
    public class DecimalNumericCoercerConverter : IValueConverter {
        /// <summary>
        /// Not implemented. Returns input upon invocation.
        /// </summary>
        /// <param name="value">Input value.</param>
        /// <param name="targetType">Output value type. (Unused.)</param>
        /// <param name="parameter">Converter parameter. (Unused.)</param>
        /// <param name="culture">Culture. (Unused.)</param>
        /// <returns>Returns input.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value ?? NullValue;
        /// <summary>
        /// Coerces value to be sure it can be represented as decimal or float.
        /// </summary>
        /// <param name="value">Input value.</param>
        /// <param name="targetType">Output value type.</param>
        /// <param name="parameter">Converter parameter. (Unused.)</param>
        /// <param name="culture">Culture. (Unused.)</param>
        /// <returns>Returns input if legal value, and returns "0" if not.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            object failValue = null;
            Type primitiveTargetType = Nullable.GetUnderlyingType(targetType);

            if (primitiveTargetType == null) {
                failValue = 0;
                primitiveTargetType = targetType;
            }

            if (value is string == false || value == null) {
                return failValue;
            }

            string v = System.Text.RegularExpressions.Regex.Replace(((string)value), "[^0-9.-]", "");

            if (primitiveTargetType == typeof(decimal)) {
                decimal n = 0;
                return decimal.TryParse(v, out n) ? n : failValue;
            }
            else if (primitiveTargetType == typeof(float)) {
                float n = 0;
                return float.TryParse(v, out n) ? n : failValue;
            }

            return value;
        }

        /// <summary>
        /// The value to output to <see cref="Convert(object, Type, object, CultureInfo)"/> if input is null;
        /// </summary>
        public object NullValue { get; set; }

    }
}