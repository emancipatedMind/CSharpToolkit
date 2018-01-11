namespace CSharpToolkit.XAML {
    using System;
    using System.Globalization;
    using System.Windows.Data;
    /// <summary>
    /// Denotes whether input is null.
    /// </summary>
    public class IsNullConverter : IValueConverter {
        /// <summary>
        /// Perform conversion of null status to bool.
        /// </summary>
        /// <param name="value">Input value.</param>
        /// <param name="targetType">Output value type. (Unused.)</param>
        /// <param name="parameter">Converter parameter. (Unused.)</param>
        /// <param name="culture">Culture. (Unused.)</param>
        /// <returns>If input is null, returns true. Otherwise, returns false.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value == null;
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
