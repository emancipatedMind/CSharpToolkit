namespace CSharpToolkit.Extensions {
    /// <summary>
    /// Extensions for use by string types.
    /// </summary>
    public static class StringExtensions {

        /// <summary>
        /// An exception-free way to get a substring. If the input string is shorter than length - startIndex, the startIndex character to end of input string is returned. If null, null is returned.
        /// </summary>
        /// <param name="input">The string to substring.</param>
        /// <param name="startIndex">Where to start substring.</param>
        /// <param name="length">Substring length.</param>
        /// <returns>Substring requested.</returns>
        public static string SafeSubstring(this string input, int startIndex, int length) =>
            Utilities.Get.Substring(input, startIndex, length);

        /// <summary>
        /// Determines whether a string is valid which is the inverse of <see cref="System.String.IsNullOrEmpty(string)"/> == false.
        /// </summary>
        /// <param name="input">The string to check.</param>
        /// <returns>Whether string is valid.</returns>
        public static bool IsValid(this string input) =>
            string.IsNullOrEmpty(input) == false;

        /// <summary>
        /// Determines whether a string is meaningful which is <see cref="System.String.IsNullOrWhiteSpace(string)"/> == false.
        /// </summary>
        /// <param name="input">The string to check.</param>
        /// <returns>Whether string is meaningful.</returns>
        public static bool IsMeaningful(this string input) =>
            string.IsNullOrWhiteSpace(input) == false;

    }
}