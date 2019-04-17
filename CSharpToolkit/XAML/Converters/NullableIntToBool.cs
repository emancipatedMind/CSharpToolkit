namespace CSharpToolkit.XAML.Converters {
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    /// <summary>
    /// Performs two way conversion between int? to bool.
    /// </summary>
    public class NullableIntToBool : IValueConverter {

        ValuesToBoolConverter _converter = new ValuesToBoolConverter {
            TrueValue = 1,
            FalseValue = 0,
            DefaultParameter = new object[] { null, 0 },
            InvertOutputToConvert = true,
        };

        /// <summary>
        /// Converts a int? to bool.
        /// </summary>
        /// <param name="value">Input value.</param>
        /// <param name="targetType">Output value type. (Unused.)</param>
        /// <param name="parameter">Converter parameter. (Unused.)</param>
        /// <param name="culture">Culture. (Unused.)</param>
        /// <returns>Returns false if incoming is int? is null or 0. Returns true if 1.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            _converter.Convert(value, null, null, null);

        /// <summary>
        /// Converts a bool to int?.
        /// </summary>
        /// <param name="value">Input value.</param>
        /// <param name="targetType">Output value type. (Unused.)</param>
        /// <param name="parameter">Converter parameter. (Unused.)</param>
        /// <param name="culture">Culture. (Unused.)</param>
        /// <returns>Returns 1 if true, and 0 if false.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            _converter.ConvertBack(value, null, null, null);

    }
}