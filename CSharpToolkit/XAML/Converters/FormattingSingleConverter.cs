namespace CSharpToolkit.XAML.Converters {
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;
    /// <summary>
    /// A ValueConverter that can format a property as a string. If incoming is DependencyProperty.UnsetValue, then the output is Binding.DoNothing.
    /// </summary>
    public class FormattingSingleConverter : IValueConverter {

        /// <summary>
        /// The formatter to use. By default, is CSharpToolkit.Utilities.Formatters.ConcatenateWithStringFormatter.
        /// </summary>
        public Utilities.Abstractions.IStringFormatter Formatter { get; set; } = new Utilities.Formatters.ConcatenateWithStringFormatter();

        /// <summary>
        /// Applies Formatter to input.
        /// </summary>
        /// <param name="value">Input to Formatter.</param>
        /// <param name="targetType">Unused. Assumed System.String.</param>
        /// <param name="parameter">Unused.</param>
        /// <param name="culture">Unused.</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value?.Equals(DependencyProperty.UnsetValue) ?? false)
                return Binding.DoNothing;

            string scrubbedText =
                Utilities.Perform.TextScrub(new[] { value?.ToString() ?? "" }, ScrubCharacters?.Select(c => System.Convert.ToChar(c)).ToArray()).FirstOrDefault() ?? "";

            string formattedText = Formatter?.Format(scrubbedText) ?? "";
            return string.IsNullOrEmpty(formattedText) ? (EmptyText == null ? scrubbedText : EmptyText) : formattedText;
        }

        /// <summary>
        /// Not implemented. Returns value.
        /// </summary>
        /// <param name="value">Not Implemented.</param>
        /// <param name="targetType">Not Implemented.</param>
        /// <param name="parameter">Not Implemented.</param>
        /// <param name="culture">Not Implemented.</param>
        /// <returns>value</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value;

        /// <summary>
        /// Dictates what text should be placed when Formatter is not set, or formatter returns empty. If null, value passed in will be returned.
        /// </summary>
        public string EmptyText { get; set; }

        /// <summary>
        /// The characters that will be scrubbed before formatting will take place.
        /// </summary>
        public int[] ScrubCharacters { get; set; } = new int[0];
    }
}