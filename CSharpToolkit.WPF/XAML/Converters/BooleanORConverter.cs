namespace CSharpToolkit.XAML.Converters {
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;
    /// <summary>
    /// Use to perform OR operation on multiple bools. If value is null or not a bool, that particular value is interpreted as false.
    /// </summary>
    public class BooleanORConverter : IMultiValueConverter {
        /// <summary>
        /// Use to perform OR operation on multiple bools. If value is null or not a bool, that particular value is interpreted as false.
        /// </summary>
        /// <param name="value">Input value.</param>
        /// <param name="targetType">Output value type. (Unused.)</param>
        /// <param name="parameter">Converter parameter. (Unused.)</param>
        /// <param name="culture">Culture. (Unused.)</param>
        /// <returns>False if input was true, or not a bool. If input was false, returns true.</returns>
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture) {
            if ((value?.Any() ?? false) == false)
                return InvertOutput;

            return value
                .Select(v => {
                    Type type = v?.GetType();
                    return type == null || (Nullable.GetUnderlyingType(type) ?? type) != typeof(bool) ?
                        false :
                        (bool)v;
                })
                .Any(v => v) != InvertOutput;
        }

        /// <summary>
        /// Method Not Implemented. Returns <paramref name="value"/> if invoked.
        /// </summary>
        /// <param name="value">Not Implemented. Throws exception if invoked.</param>
        /// <param name="targetType">Not Implemented. Throws exception if invoked.</param>
        /// <param name="parameter">Not Implemented. Throws exception if invoked.</param>
        /// <param name="culture">Not Implemented. Throws exception if invoked.</param>
        /// <returns>Method Not Implemented. Returns <paramref name="value"/> if invoked.</returns>
        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture) => new[] { value };

        /// <summary>
        /// Dictates whether to invert the output.
        /// </summary>
        public bool InvertOutput { get; set; } = false;

    }
}