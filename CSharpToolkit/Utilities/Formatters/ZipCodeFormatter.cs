namespace CSharpToolkit.Utilities.Formatters {
    using Abstractions;
    /// <summary>
    /// Used to format zip codes.
    /// </summary>
    public class ZipCodeFormatter : IStringFormatter {

        System.Text.RegularExpressions.Regex _numericCharacterPreserver = new System.Text.RegularExpressions.Regex("[^0-9]");

        /// <summary>
        /// Expects a single argument. If that single argument contains 9 numbers, all other characters are removed, and a string is returned in the following format : 'xxxxx-xxxx'.
        /// </summary>
        /// <param name="text">Text to format.</param>
        /// <returns>Formatted zip code if input was in expected format, and returns input if not.</returns>
        public string Format(params string[] text) {
            if (ValidationFails(text))
                return "";

            string input = _numericCharacterPreserver.Replace(text[0], "");

            if (input.Length != 9)
                return text[0];

            return input.Insert(5, "-");
        }

        bool ValidationFails(string[] text) =>
            text == null || text.Length < 1 || string.IsNullOrEmpty(text[0]);
    }
}