using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace CSharpToolkit.XAML.Converters
{
    /// <summary>
    /// Used for year conversions.
    /// </summary>
    public class YearConverter : IValueConverter
    {
        /// <summary>
        /// Used to convert viewmodel value to xaml string.
        /// </summary>
        /// <param name="value">Input value.</param>
        /// <param name="targetType">Output value type. (Unused.)</param>
        /// <param name="parameter">Converter parameter. (Unused.)</param>
        /// <param name="culture">Culture. (Unused.)</param>
        /// <returns>Returns string as yyyy.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            bool valueIsInvalid = value == null || value is int == false || value is int? == false;
            if (valueIsInvalid)
                return value ?? "";

            return value.ToString();
            
        }

        /// <summary>
        /// Used to convert user input in Xaml textbox to viewmodel  year as int.
        /// </summary>
        /// <param name="value">Input value.</param>
        /// <param name="targetType">Output value type. (Unused.) </param>
        /// <param name="parameter">Converter parameter. (Unused.)</param>
        /// <param name="culture">Culture. (Unused.)</param>
        /// <returns>Returns null if value is empty, valid DateTime if parse succeeds, and value if parse fails.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string year = value.ToString();

            //Coming from textbox, value must be "" or 1-4 digit int
            if (year.Length == 0) return null;

            //value must be 0-9999
            if (year.Length == 1) return "200" + year;
            if (year.Length == 2) return "20" + year;
            if (year.Length == 3) return "2" + year;

            return year;
        }
    }
}
