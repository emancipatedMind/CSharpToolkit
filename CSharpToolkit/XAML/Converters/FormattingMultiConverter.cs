namespace CSharpToolkit.XAML.Converters {
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Linq;
    /// <summary>
    /// An implementation of <see cref="IMultiValueConverter"/> that can format multiple properties as strings.
    /// </summary>
    public class FormattingMultiConverter : IMultiValueConverter {

        /// <summary>
        /// The formatter to use. By default, is <see cref="Utilities.Formatters.ConcatenateWithStringFormatter"/>.
        /// </summary>
        public Utilities.Abstractions.IStringFormatter Formatter { get; set; } = new Utilities.Formatters.ConcatenateWithStringFormatter();
        
        /// <summary>
        /// Applies Formatter to inputs.
        /// </summary>
        /// <param name="values">Input to Formatter.</param>
        /// <param name="targetType">Unused. Assumed <see cref="System.String"/>.</param>
        /// <param name="parameter">Unused.</param>
        /// <param name="culture">Unused.</param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            if (values?.All(v => v == null || v.Equals("") || v.Equals(DependencyProperty.UnsetValue)) ?? true)
                return DependencyProperty.UnsetValue;

            var stringValues = values?.Select(v => 
                v?.Equals(DependencyProperty.UnsetValue) ?? true ?
                    "" :
                    Utilities.Perform.TextScrub(new[] { v?.ToString() ?? "" }, ScrubCharacters?.Select(c => System.Convert.ToChar(c)).ToArray()).FirstOrDefault() ?? ""
            )
            .ToArray() ?? new string[0];
            string formattedText = Formatter?.Format(stringValues) ?? "";
            return string.IsNullOrEmpty(formattedText) ? (EmptyText == null ? "" : EmptyText) : formattedText;
        }

        /// <summary>
        /// Method Not Implemented. Returns <paramref name="value"/> if invoked.
        /// </summary>
        /// <param name="value">Not Implemented.</param>
        /// <param name="targetTypes">Not Implemented.</param>
        /// <param name="parameter">Not Implemented.</param>
        /// <param name="culture">Not Implemented.</param>
        /// <returns>Method Not Implemented. Returns <paramref name="value"/> if invoked.</returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => new[] { value };

        /// <summary>
        /// Dictates what text should be placed when Formatter is not set, or formatter returns empty. If null, <see cref="String"/> is returned.
        /// </summary>
        public string EmptyText { get; set; }


        /// <summary>
        /// The characters that will be scrubbed before formatting will take place.
        /// </summary>
        public int[] ScrubCharacters { get; set; } = new int[0];
    }
}