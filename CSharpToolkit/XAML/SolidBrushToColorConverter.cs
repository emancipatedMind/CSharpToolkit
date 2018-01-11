namespace CSharpToolkit.XAML {
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    /// <summary>
    /// Use to convert SolidColorBrush to color.
    /// </summary>
    public class SolidBrushToColorConverter : IValueConverter {
        /// <summary>
        /// Perform conversion from Brush to Color.
        /// </summary>
        /// <param name="value">Input value.</param>
        /// <param name="targetType">Output value type. (Unused.)</param>
        /// <param name="parameter">Converter parameter. (Unused.)</param>
        /// <param name="culture">Culture. (Unused.)</param>
        /// <returns>Color of brush. If input was not a SolidColorBrush, return DependencyProperty.UnsetValue</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is SolidColorBrush) {
                return ((SolidColorBrush)value).Color;
            }
            return DependencyProperty.UnsetValue;
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