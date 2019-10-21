namespace CSharpToolkit.Utilities.Formatters {
    using Abstractions;
    using Extensions;
    using System.Linq;
    /// <summary>
    /// Concatenates strings together using a Link.
    /// </summary>
    public class ConcatenateWithStringFormatter : IStringFormatter {

        /// <summary>
        /// Formats inputs by concatenating with Link.
        /// </summary>
        /// <param name="text">Text to format.</param>
        /// <returns>Inputs concatenated with Link.</returns>
        public string Format(params string[] text) {
            var formattedInputs = text.Where(t => t.IsMeaningful());
            return string.Join(Link ?? "", formattedInputs);
        }

        /// <summary>
        /// Details string to link inputs with.
        /// </summary>
        public string Link { get; set; } = " ";
    }
}