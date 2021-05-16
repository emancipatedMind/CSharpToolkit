namespace CSharpToolkit.Utilities.Formatters {
    using System.Linq;
    using System.Text.RegularExpressions;
    /// <summary>
    /// An implementation of the CSharpToolkit.Utilities.Formatters.Abstractions.IStringFormatter that uses a regular expression.
    /// </summary>
    public class RegExFormatter : Abstractions.IStringFormatter {
        /// <summary>
        /// Takes text, links them with InputLink, finds all matches of RegularExpression, links the matches with OutputLink, and outputs result.
        /// </summary>
        /// <param name="text">Text to format.</param>
        /// <returns>If any stage produces an empty string, or there are no matches, EmptyText is returned. Otherwise, the result of operation is.</returns>
        public string Format(params string[] text) {
            string input = string.Join(InputLink, Perform.NullAndWhitespaceReplace(text));
            if (string.IsNullOrEmpty(input))
                return EmptyText;
            if (string.IsNullOrWhiteSpace(RegularExpression))
                return EmptyText;
            var regularExpression = new Regex(RegularExpression);

            string outputCandidate =
                string.Join(OutputLink, regularExpression.Match(input).Groups.Cast<Group>().Skip(1).Select(m => m.Value));
            return string.IsNullOrWhiteSpace(outputCandidate) ?
                EmptyText :
                outputCandidate;
        }

        /// <summary>
        /// The text for when: the input is null; contains no members; all members evaluate to String.Empty, or null; Regular Expression is empty; Regular Expression produces no matches.
        /// </summary>
        public string EmptyText { get; set; } = "";
        /// <summary>
        /// The Regular Expression to use on text.
        /// </summary>
        public string RegularExpression { get; set; } = "";
        /// <summary>
        /// The text to use to link incoming members.
        /// </summary>
        public string InputLink { get; set; } = "";
        /// <summary>
        /// The text to use to link matches.
        /// </summary>
        public string OutputLink { get; set; } = "";
    }
}