namespace CSharpToolkit.XAML.Converters {
    using System;
    using System.Globalization;
    using System.Windows.Data;
    /// <summary>
    /// Used to verify that value can be represented as byte, short, int, or long.
    /// </summary>
    public class NumericCoercerConverter : IValueConverter {
        /// <summary>
        /// Not implemented. Returns input upon invocation.
        /// </summary>
        /// <param name="value">Input value.</param>
        /// <param name="targetType">Output value type. (Unused.)</param>
        /// <param name="parameter">Converter parameter. (Unused.)</param>
        /// <param name="culture">Culture. (Unused.)</param>
        /// <returns>Returns input.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value;
        /// <summary>
        /// Coerces value to be sure it can be represented as byte, short, int, or long.
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

            decimal decimalValue;
            bool parseFails =
                decimal
                    .TryParse((string)value, out decimalValue) == false;

            if (parseFails) 
                return failValue;

            if (primitiveTargetType == typeof(int)) {
                if (decimalValue > int.MaxValue)
                    return int.MaxValue;
                if (decimalValue < int.MinValue)
                    return int.MinValue;
                return System.Convert.ToInt32(decimalValue);
            }
            else if (primitiveTargetType == typeof(byte)) {
                if (decimalValue > byte.MaxValue)
                    return byte.MaxValue;
                if (decimalValue < byte.MinValue)
                    return byte.MinValue;
                return System.Convert.ToByte(decimalValue);
            }
            else if (primitiveTargetType == typeof(long)) {
                if (decimalValue > long.MaxValue)
                    return long.MaxValue;
                if (decimalValue < long.MinValue)
                    return long.MinValue;
                return System.Convert.ToInt64(decimalValue);
            }
            else if (primitiveTargetType == typeof(short)) {
                if (decimalValue > short.MaxValue)
                    return short.MaxValue;
                if (decimalValue < short.MinValue)
                    return short.MinValue;
                return System.Convert.ToInt16(decimalValue);
            }

            return decimalValue;
        }

    }
}