namespace CSharpToolkit.XAML.Converters {
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Windows.Data;
    /// <summary>
    /// Used for datetime conversions.
    /// </summary>
    public class DateConverter : IValueConverter {

        static string DefaultTimeFormat = "MM/dd/yyyy";
        /// <summary>
        /// Array of formats accepted for the date.
        /// </summary>
        static string[] _formats = {
            "M/d",
            "MMd",
            "ddM",
            "M/dd",
            "M/d/yyyy",
            "d/M/yyyy",
            "MMdd",
            "ddMM",
            "MM/dd",
            "dd/MM",
            "Mddyyyy",
            "ddMyyyy",
            "M/dd/yyyy",
            "dd/M/yyyy",
            "MMddyyyy",
            "ddMMyyyy",
            "MM/dd/yyyy",
            "dd/MM/yyyy",
        };

        string _dateFormat;

        /// <summary>
        /// Used to convert user input into valid date format.
        /// </summary>
        /// <param name="value">Input value.</param>
        /// <param name="targetType">Output value type. (Unused.)</param>
        /// <param name="parameter">Converter parameter. (Unused.)</param>
        /// <param name="culture">Culture. (Unused.)</param>
        /// <returns>Returns DateTime formatted from user input.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            bool valueIsInvalid = value == null || value is DateTime == false || value is DateTime? == false;
            if (valueIsInvalid)
                return value;
            var format = DateFormat + (string.IsNullOrWhiteSpace(TimeFormat) ? "" : $" {TimeFormat}");

            return ((DateTime)value).ToString(format, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Used to parse user input, and convert to valid DateTime.
        /// </summary>
        /// <param name="value">Input value.</param>
        /// <param name="targetType">Output value type. (Unused.) </param>
        /// <param name="parameter">Converter parameter. (Unused.)</param>
        /// <param name="culture">Culture. (Unused.)</param>
        /// <returns>Returns null if value is empty, valid DateTime if parse succeeds, and value if parse fails.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            string date = value as string;

            // Detect if input is empty, if so, a cleared field is the assumption.
            if (string.IsNullOrWhiteSpace(date))
                return null;

            // Before any testing, the date will be trimmed.
            // Since a user is allowed to input a date like this 02/08-2006, all dashes are replaced with forward slashes.
            // This also decreases the number of formats that must be tested for.
            date = date.Trim().Replace("-", "/");

            // Here the date is length corrected :
            //     > If it's less than three characters, the assumption is that only the day has been entered, and the current month is prepended.
            //     > If it's all numeric characters, only the first eight are captured.
            //     > If it contains a non-numeric character, only the first ten are captured to account for 8 digits, and 2 separators.
            if (date.Length < 3)
                date = DateTime.Now.ToString("MM") + date;
            int dateMaxLength = Regex.IsMatch(date, @"^\d+$") ? 8 : 10;
            date = CSharpToolkit.Extensions.StringExtensions.SafeSubstring(date, 0, dateMaxLength);

            // If date has a non four character year, we assume that the user would like this to be offset
            // by the year 2000. So, 8, 08, 008 is interpreted as 2008. Here that offset is applied if the
            // non four character year is detected.
            string pattern = @"^((?:\d{1,2}[/-]\d{1,2}[/-])|(?:\d{4}))(\d{1,3})$";
            Match dateGroups = Regex.Match(date, pattern);
            if (dateGroups.Success) {
                int year = int.Parse(dateGroups.Groups[2].Value) + 2000;
                date = dateGroups.Groups[1].Value + year.ToString();
            }

            // The permissible formats are passed, and if the parse is successful, the produced
            // DateTime struct is passed back.
            // If the parse is unsuccessful, the original value entered is returned.
            DateTime dateOut;
            bool parseWasSuccessful =
                DateTime.TryParseExact(date,
                    _formats,
                    new CultureInfo("en-US"),
                    DateTimeStyles.AssumeLocal,
                    out dateOut);

            return parseWasSuccessful ? dateOut : value;
        }

        /// <summary>
        /// Format of Date portion.
        /// </summary>
        public string DateFormat {
            get { return string.IsNullOrWhiteSpace(_dateFormat) ? DefaultTimeFormat : _dateFormat; }
            set { _dateFormat = value; }
        }

        /// <summary>
        /// Format of Time portion.
        /// </summary>
        public string TimeFormat { get; set; } = "";

    }
}