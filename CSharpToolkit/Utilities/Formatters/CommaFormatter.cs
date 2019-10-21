namespace CSharpToolkit.Utilities.Formatters {
    using Abstractions;
    /// <summary>
    /// Places the string ', ' between all elements.
    /// </summary>
    public class CommaFormatter : IStringFormatter {

        static ConcatenateWithStringFormatter formatter = new ConcatenateWithStringFormatter { Link = ", " };

        static CommaFormatter _instance;
        public static CommaFormatter Instance => _instance ?? (_instance = new CommaFormatter());

        CommaFormatter() { }
        public string Format(params string[] text) => formatter.Format(text);

    }
}
