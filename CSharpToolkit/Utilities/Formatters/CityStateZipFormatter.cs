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
        /// Formats city, state, and zip into a single line.
        /// </summary>
        /// <param name="city"></param>
        /// <param name="state"></param>
        /// <param name="zip"></param>
        /// <returns></returns>
        public string FormatCityStateZip(string city, string state, string zip) =>
            Format(new[] { city, state, zip });

        /// <summary>
        /// The formatter used to format the zip code.
        /// </summary>
        public IStringFormatter ZipCodeFormatter { get; set; } = new ZipCodeFormatter();

        private string TestAndCombine(string input, string secondInput, string proposedJoinString) {

            string actualJoinString =
                string.IsNullOrWhiteSpace(input) == false && string.IsNullOrWhiteSpace(secondInput) == false ?
                proposedJoinString :
                "";

            return input + actualJoinString + secondInput;
        } 

    }
}