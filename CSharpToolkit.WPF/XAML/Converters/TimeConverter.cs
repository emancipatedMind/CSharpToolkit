namespace CSharpToolkit.XAML.Converters {
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Windows.Data;
    /// <summary>
    /// Used for time conversions.
    /// </summary>
    public class TimeConverter : IValueConverter {

        static string DefaultTimeFormat = @"hh\:mm";

        /// <summary>
        /// Array of formats accepted for the date.
        /// </summary>
        static string[] _formats = {
            //"h",
            //"h:m",
            //"h:mm",
            //"hh",
            //"hh:m",
            //"hh:mm",
            "hhmm",
        };

        string _timeFormat;

        /// <summary>
        /// Used to convert user input into valid date format.
        /// </summary>
        /// <param name="value">Input value.</param>
        /// <param name="targetType">Output value type. (Unused.)</param>
        /// <param name="parameter">Converter parameter. (Unused.)</param>
        /// <param name="culture">Culture. (Unused.)</param>
        /// <returns>Returns DateTime formatted from user input.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value == null || value is TimeSpan == false || value is TimeSpan? == false ?
                value :
                ((TimeSpan)value).ToString(TimeFormat, CultureInfo.InvariantCulture);

        /// <summary>
        /// Used to parse user input, and convert to valid DateTime.
        /// </summary>
        /// <param name="value">Input value.</param>
        /// <param name="targetType">Output value type. (Unused.)</param>
        /// <param name="parameter">Converter parameter. (Unused.)</param>
        /// <param name="culture">Culture. (Unused.)</param>
        /// <returns>Returns null if value is empty, valid DateTime if parse succeeds, and value if parse fails.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            string time = value as string;

            // Detect if input is empty, if so, a cleared field is the assumption.
            if (string.IsNullOrWhiteSpace(time))
                return null;

            time = time.Replace(":", "").PadLeft(4, '0');

            // The permissible formats are passed, and if the parse is successful, the produced
            // TimeSpan struct is passed back.
            // If the parse is unsuccessful, the original value entered is returned.
            TimeSpan timeOut;
            bool parseWasSuccessful =
                TimeSpan.TryParseExact(time,
                    _formats,
                    CultureInfo.InvariantCulture,
                    TimeSpanStyles.None,
                    out timeOut);

            return parseWasSuccessful ? timeOut : value;
        }

        /// <summary>
        /// Format of TimeSpan.
        /// </summary>
        public string TimeFormat {
            get { return string.IsNullOrWhiteSpace(_timeFormat) ? DefaultTimeFormat : _timeFormat; }
            set { _timeFormat = value; }
        }

    }
}