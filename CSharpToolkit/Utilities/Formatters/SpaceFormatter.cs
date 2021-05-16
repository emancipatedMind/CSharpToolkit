namespace CSharpToolkit.Utilities.Formatters {
    using Abstractions;
    /// <summary>
    /// Places the string ' ' between all elements.
    /// </summary>
    public class SpaceFormatter : IStringFormatter {

        static ConcatenateWithStringFormatter formatter = new ConcatenateWithStringFormatter();

        static SpaceFormatter _instance;
        public static SpaceFormatter Instance => _instance ?? (_instance = new SpaceFormatter());

        SpaceFormatter() { }
        public string Format(params string[] text) => formatter.Format(text);

    }
}
