using CSharpToolkit.Utilities.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpToolkit.Utilities.Formatters
{
    /// <summary>
    /// Concatenates strings together using a hypen.
    /// </summary>
    public class ConcatenateWithHyphenFormatter : IStringFormatter
    {
        /// <summary>
        /// Formats inputs by concatenating with Hyphen.
        /// </summary>
        /// <param name="text">Text to format.</param>
        /// <returns>Inputs concatenated with a Hyphen.</returns>
        public string Format(params string[] text)
        {
            var formattedInputs = text.Where(t => string.IsNullOrWhiteSpace(t) == false);
            return string.Join(Hyphen ?? "", formattedInputs);
        }

        /// <summary>
        /// Details string to link inputs with.
        /// </summary>
        public string Hyphen { get; set; } = "-";
    }
}
