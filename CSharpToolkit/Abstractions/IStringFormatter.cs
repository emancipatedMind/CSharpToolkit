namespace CSharpToolkit.Abstractions {
    /// <summary>
    /// Implemented by a class who can format strings.
    /// </summary>
    public interface IStringFormatter {
        /// <summary>
        /// Format strings.
        /// </summary>
        /// <param name="text">The strings that need to be formatted.</param>
        /// <returns>Formatted string.</returns>
        string Format(params string[] text);
    }
}