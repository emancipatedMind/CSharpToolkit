namespace CSharpToolkit.XAML.Converters {
    using System;
    using System.Globalization;
    using System.Windows.Data;
    /// <summary>
    /// Used to filter out certain values, and replace with another object.
    /// </summary>
    public class FilterConverter : IValueConverter {
        ValuesToBoolConverter _converter = new ValuesToBoolConverter();

        /// <summary>
        /// Determines whether value is a value that is defined by parameter. If parameter is null, DefaultParameter is consulted.
        /// </summary>
        /// <param name="value">Value to test.</param>
        /// <param name="targetType">Unused.</param>
        /// <param name="parameter">Values to test. Can be single object, or an array of objects.</param>
        /// <param name="culture">Unused.</param>
        /// <returns>Returns FilterValue or value depending if value is found, and status of InvertOutputToConvert.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            bool convertSuccess = (bool)_converter.Convert(value, targetType, parameter, culture);
            return convertSuccess ? FilterValue : value; 
        }

        /// <summary>
        /// Unused. Returns value upon invocation.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="targetType">Unused  method parameter.</param>
        /// <param name="parameter">Unused  method parameter.</param>
        /// <param name="culture">Unused method parameter.</param>
        /// <returns>Unused. Throws exception if invoked.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value;

        /// <summary>
        /// The parameter that is used if parameter passed to Convert method is null.
        /// </summary>
        public object DefaultParameter { get { return _converter.DefaultParameter; } set { _converter.DefaultParameter = value; } }
        /// <summary>
        /// The value to output if the value is found, and InvertOutputToConvert is false. 
        /// </summary>
        public object FilterValue { get; set; } = Binding.DoNothing;
        /// <summary>
        /// Determines if output of Convert method should be inverted or not.
        /// </summary>
        public bool InvertOutputToConvert { get { return _converter.InvertOutputToConvert; } set { _converter.InvertOutputToConvert = value; } }
    }
}