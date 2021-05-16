namespace CSharpToolkit.XAML.Converters {
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    /// <summary>
    /// Used to convert <see cref="Color"/> to <see cref="SolidColorBrush"/>.
    /// </summary>
    public class ColorToSolidBrushConverter : IValueConverter {
        /// <summary>
        /// Perform conversion from <see cref="Color"/> to <see cref="SolidColorBrush"/>.
        /// </summary>
        /// <param name="value">Input value.</param>
        /// <param name="targetType">Output value type. (Unused.)</param>
        /// <param name="parameter">Converter parameter. (Unused.)</param>
        /// <param name="culture">Culture. (Unused.)</param>
        /// <returns>New SolidColorBrush. If input was not a Color, return DependencyProperty.UnsetValue</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is Color ?
                new SolidColorBrush((Color)value) :
                DependencyProperty.UnsetValue;

        /// <summary>
        /// Not Implemented. Returns <paramref name="value"/> if invoked.
        /// </summary>
        /// <param name="value">Not Implemented. Throws exception if invoked.</param>
        /// <param name="targetType">Not Implemented. Throws exception if invoked.</param>
        /// <param name="parameter">Not Implemented. Throws exception if invoked.</param>
        /// <param name="culture">Not Implemented. Throws exception if invoked.</param>
        /// <returns>Not Implemented. Throws exception if invoked.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value;
    }
}