namespace CSharpToolkit.XAML.Converters {
    using System;
    using System.Globalization;
    using System.Windows.Data;
    /// <summary>
    /// Denotes whether input is null.
    /// </summary>
    public class IsNullConverter : IValueConverter {

        ValuesToBoolConverter _converter = new ValuesToBoolConverter();

        /// <summary>
        /// Perform conversion of null status to bool.
        /// </summary>
        /// <param name="value">Input value.</param>
        /// <param name="targetType">Output value type. (Unused.)</param>
        /// <param name="parameter">Converter parameter. (Unused.)</param>
        /// <param name="culture">Culture. (Unused.)</param>
        /// <returns>If input is null, returns true. Otherwise, returns false.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            _converter.Convert(value, targetType, null, culture);

        /// <summary>
        /// Not Implemented. Returns Binding.DoNothing if invoked.
        /// </summary>
        /// <param name="value">Not Implemented. Returns Binding.DoNothing if invoked.</param>
        /// <param name="targetType">Not Implemented. Returns Binding.DoNothing if invoked.</param>
        /// <param name="parameter">Not Implemented. Returns Binding.DoNothing if invoked.</param>
        /// <param name="culture">Not Implemented. Returns Binding.DoNothing if invoked.</param>
        /// <returns>Not Implemented. Returns Binding.DoNothing if invoked.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            _converter.ConvertBack(value, targetType, parameter, culture);

        /// <summary>
        /// Dictates whether to invert the output of convert.
        /// </summary>
        public bool InvertOutput {
            get { return _converter.InvertOutputToConvert; }
            set { _converter.InvertOutputToConvert = value; }
        }
    }
}