namespace CSharpToolkit.XAML.Converters {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;
    using System.Windows.Markup;
    /// <summary>
    /// Used to convert from bool to other values.
    /// </summary>
    public class ValuesToBoolConverter : IValueConverter {

        /// <summary>
        /// Determines whether value is a value that is defined by parameter. If parameter is null, DefaultParameter is consulted.
        /// </summary>
        /// <param name="value">Value to test.</param>
        /// <param name="targetType">Unused.</param>
        /// <param name="parameter">Values to test. Can be single object, or an array of objects.</param>
        /// <param name="culture">Unused.</param>
        /// <returns>Returns a bool which indicates if value was contained inside consulted parameter.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            parameter = parameter ?? DefaultParameter;

            Type parameterType = parameter?.GetType();

            var items = new List<object>();
            if (parameterType?.IsArray == true)
                items.AddRange((object[]) parameter);
            else
                items.Add(parameter);

            bool output =
                value == null ?
                    items.Contains(null) :
                    items.Cast<object>().Any(obj => obj?.Equals(value) ?? false);

            return output != InvertOutputToConvert;
        }

        /// <summary>
        /// Used to convert bool to TrueValue or FalseValue.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="targetType">Unused  method parameter.</param>
        /// <param name="parameter">Unused  method parameter.</param>
        /// <param name="culture">Unused method parameter.</param>
        /// <returns>If value is true, returns object contained in TrueValue. Anything else returns FalseValue.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            value?.Equals(true) ?? false ?
                TrueValue :
                FalseValue;

        /// <summary>
        /// Is object returned when the ConvertBack method is passed true through value.
        /// </summary>
        public object TrueValue { get; set; } = Binding.DoNothing;
        /// <summary>
        /// Is object returned when the ConvertBack method is passed anything but true.
        /// </summary>
        public object FalseValue { get; set; } = Binding.DoNothing;
        /// <summary>
        /// The parameter that is used if parameter passed to Convert method is null.
        /// </summary>
        public object DefaultParameter { get; set; } = null;
        /// <summary>
        /// Determines if output of Convert method should be inverted or not.
        /// </summary>
        public bool InvertOutputToConvert { get; set; } = false;

    }
}