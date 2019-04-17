namespace CSharpToolkit.Utilities.Formatters {
    using Abstractions;
    using System.Linq;
    /// <summary>
    /// Returns first string that isn't null, empty, or just whitespace. If no legal string, returns string.Empty.
    /// </summary>
    public class FirstLegalTextFormatter : IStringFormatter {
        /// <summary>
        /// Returns first string that isn't null or, empty, or just whitespace. If no legal string, returns string.Empty.
        /// </summary>
        /// <param name="text">Input text.</param>
        /// <returns>Returns first string that isn't null or, empty, or just whitespace. If no legal string, returns string.Empty.</returns>
        public string Format(params string[] text) {
            string[] washedText = Perform.NullAndWhitespaceReplace(text);
            return washedText.FirstOrDefault(t => string.IsNullOrEmpty(t) == false) ?? "";
        }
    }
}