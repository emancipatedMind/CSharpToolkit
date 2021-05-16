namespace CSharpToolkit.XAML.Converters {
    using System;
    using System.Globalization;
    using System.Windows.Data;
    /// <summary>
    /// Use to perform NOT operation on bool.
    /// </summary>
    public class BooleanNotConverter : IValueConverter {
        /// <summary>
        /// Perform conversion from True to False or vice versa.
        /// </summary>
        /// <param name="value">Input value.</param>
        /// <param name="targetType">Output value type. (Unused.)</param>
        /// <param name="parameter">Converter parameter. (Unused.)</param>
        /// <param name="culture">Culture. (Unused.)</param>
        /// <returns>False if input was true, or not a bool. If input was false, returns true.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if ((value is bool) && ((bool)value) == false) return true;
            else return false;
        }

        /// <summary>
        /// Not Implemented. Throws exception if invoked.
        /// </summary>
        /// <param name="value">Not Implemented. Throws exception if invoked.</param>
        /// <param name="targetType">Not Implemented. Throws exception if invoked.</param>
        /// <param name="parameter">Not Implemented. Throws exception if invoked.</param>
        /// <param name="culture">Not Implemented. Throws exception if invoked.</param>
        /// <returns>Not Implemented. Throws exception if invoked.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
