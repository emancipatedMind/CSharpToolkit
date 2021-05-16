namespace CSharpToolkit.Utilities.Formatters {
    using Extensions;
    using System;
    using System.Linq;

    /// <summary>
    /// An implementation of the <see cref="Abstractions.IStringFormatter"/> which takes first element passed, and applies it to <see cref="ToStringFormatString"/>.
    /// </summary>
    public class ToStringFormatter : Abstractions.IStringFormatter {

        public string Format(params string[] text) {
            string first = text.FirstOrDefault();

            return string.IsNullOrWhiteSpace(first) ?
                "" :
                string.Format(ToStringFormatString ?? "{0}", first);
        }

        public string ToStringFormatString { get; set; }

    }
}
