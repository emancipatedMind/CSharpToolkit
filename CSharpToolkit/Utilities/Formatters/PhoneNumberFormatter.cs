namespace CSharpToolkit.Utilities.Formatters {
    using Abstractions;
    using System.Linq;
    using System.Text.RegularExpressions;
    /// <summary>
    /// Used to format a phone number.
    /// </summary>
    public class PhoneNumberFormatter : IStringFormatter {

        /// <summary>
        /// Expects one argument. Used to format a phone number.
        /// </summary>
        /// <param name="text">Expects one argument. Text to format into phone number.</param>
        /// <returns>Returns (781) 555 - 1234 if ten digits, 555 - 1234 if seven digits, <see cref="EmptyOrNull"/> if input is null, or has a length of zero and otherwise, returns input.</returns>
        public string Format(params string[] text) {

            string input = text.ElementAtOrDefault(0) ?? "";
            if (string.IsNullOrEmpty(input)) return EmptyOrNull;

            if (Regex.IsMatch(input, @"^\d+$")) {
                if (input.Length == 10)
                    return $"({input.Substring(0, 3)}) {input.Substring(3, 3)} - {input.Substring(6)}";
                if (input.Length == 7)
                    return $"{input.Substring(0, 3)} - {input.Substring(3)}";
            }

            return input;
        }

        /// <summary>
        /// Specifies string to display when value is null or empty. string.Empty by default.
        /// </summary>
        public string EmptyOrNull { get; set; } = string.Empty;
    }
}