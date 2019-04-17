namespace CSharpToolkit.Utilities.Formatters {
    using Abstractions;
    using System.Linq;
    /// <summary>
    /// Used to format names by combining with a white space.
    /// </summary>
    public class NameFormatter : IStringFormatter {

        /// <summary>
        /// Expects two arguments. Ignores others. Used to format names.
        /// </summary>
        /// <param name="text">Text to format.</param>
        /// <returns>If both arguments are null, empty or whitespace, returns string.Empty. If second argument is missing, just returns first. If both are present, combines with a whitespace.</returns>
        public string Format(params string[] text) {

            string[] washedText = Perform.NullAndWhitespaceReplace(text);

            if (washedText.Any() == false) {
                return "";
            }

            if (washedText.Length == 1) {
                return washedText[0];
            }

            string name = $"{washedText[0]} {washedText[1]}";

            return string.IsNullOrWhiteSpace(name) ? "" : name;
        }

        /// <summary>
        /// Used to format names by combining with a white space.
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns>If both names are null, empty or whitespace, returns string.Empty. If last name is missing, just returns first name. If both are present, combines with a whitespace.</returns>
        public string FormatName(string firstName, string lastName) =>
            Format(new[] { firstName, lastName });
    }
}