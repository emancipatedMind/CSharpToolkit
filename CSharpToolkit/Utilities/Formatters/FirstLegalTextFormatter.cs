namespace CSharpToolkit.Utilities.Formatters {
    using Abstractions;
    using System.Linq;
    using System;

    /// <summary>
    /// Returns first string that isn't null, empty, or just whitespace. If no legal string, returns string.Empty.
    /// </summary>
    public class FirstLegalTextFormatter : IStringFormatter {

        static FirstLegalTextFormatter _instance;
        public static FirstLegalTextFormatter Instance => _instance ?? (_instance = new FirstLegalTextFormatter());

        private FirstLegalTextFormatter() { }

        /// <summary>
        /// Returns first string that isn't null or, empty, or just whitespace. If no legal string, returns string.Empty.
        /// </summary>
        /// <param name="text">Input text.</param>
        /// <returns>Returns first string that isn't null or, empty, or just whitespace. If no legal string, returns string.Empty.</returns>
        public string Format(params string[] text) =>
            Perform.NullAndWhitespaceReplace(text).FirstOrDefault(t => string.IsNullOrEmpty(t) == false) ?? "";

    }
}