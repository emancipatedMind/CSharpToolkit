namespace CSharpToolkit.Utilities.Formatters {
    using System.Linq;
    using Abstractions;
    using Extensions;

    public class NameExtensionFormatter : IStringFormatter {
        public IStringFormatter NameFormatter { get; set; } = new NameFormatter();
        public string ExtensionPrependage { get; set; } = "x";

        public string Format(params string[] text) {
            string[] washedText = Perform.NullAndWhitespaceReplace(text);
            return (NameFormatter?.Format(washedText) ?? "") + (washedText.Length > 2 && washedText[2].IsMeaningful() ? $" {ExtensionPrependage}{washedText[2] }" : "");
        }
    }
}