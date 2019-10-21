namespace CSharpToolkit.Utilities.Formatters {
    using Abstractions;
    using System.Linq;
    /// <summary>
    /// Expects three arguments. Used to format a city, state, and zip into single line.
    /// </summary>
    public class CityStateZipFormatter : IStringFormatter {

        /// <summary>
        /// Used to format a city, state, and zip into a single line.
        /// </summary>
        /// <param name="text">Expects city, state, and then zip.</param>
        /// <returns>City, state, and zip formatted into single line.</returns>
        public string Format(params string[] text) {
            var washedText = Perform.NullAndWhitespaceReplace(text);

            string city = washedText.ElementAtOrDefault(0) ?? "";
            string state = washedText.ElementAtOrDefault(1) ?? "";
            string zip = ZipCodeFormatter.Format(washedText.ElementAtOrDefault(2) ?? "");

            string output = "";
            output = TestAndCombine(output, city, "");
            output = TestAndCombine(output, state, ", ");
            output = TestAndCombine(output, zip, " ");
            return output;
        }

        /// <summary>
        /// The formatter used to format the zip code.
        /// </summary>
        public IStringFormatter ZipCodeFormatter { get; set; } = Formatters.ZipCodeFormatter.Instance;

        static string TestAndCombine(string input, string secondInput, string proposedJoinString) {

            string actualJoinString =
                string.IsNullOrWhiteSpace(input) == false && string.IsNullOrWhiteSpace(secondInput) == false ?
                proposedJoinString :
                "";

            return input + actualJoinString + secondInput;
        } 

    }
}