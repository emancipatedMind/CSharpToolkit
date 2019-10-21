using CSharpToolkit.Utilities.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpToolkit.Utilities.Formatters {
    /// <summary>
    /// Concatenates strings together using a hypen.
    /// </summary>
    public class ConcatenateWithHyphenFormatter : IStringFormatter {

        static ConcatenateWithHyphenFormatter _instance;
        public static ConcatenateWithHyphenFormatter Instance => _instance ?? (_instance = new ConcatenateWithHyphenFormatter());

        ConcatenateWithHyphenFormatter() { }
        static ConcatenateWithStringFormatter concatenator = new ConcatenateWithStringFormatter { Link = "-" };

        /// <summary>
        /// Formats inputs by concatenating with Hyphen.
        /// </summary>
        /// <param name="text">Text to format.</param>
        /// <returns>Inputs concatenated with a Hyphen.</returns>
        public string Format(params string[] text) => concatenator.Format(text);

    }
}
