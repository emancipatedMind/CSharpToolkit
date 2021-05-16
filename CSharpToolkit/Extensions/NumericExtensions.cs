namespace CSharpToolkit.Extensions {
    /// <summary>
    /// Extensions for use by numeric types.
    /// </summary>
    public static class NumericExtensions {

        /// <summary>
        /// Used to coerce an int into a range.
        /// </summary>
        /// <param name="source">Number to coerce.</param>
        /// <param name="minimum">Minimum allowed.</param>
        /// <param name="maximum">Maximum allowed.</param>
        /// <returns>Coerced number.</returns>
        public static int Coerce(this int source, int minimum, int maximum) {
            if (source < minimum)
                return minimum;
            if (source > maximum)
                return maximum;
            return source;
        }
        /// <summary>
        /// Used to coerce a short into a range.
        /// </summary>
        /// <param name="source">Number to coerce.</param>
        /// <param name="minimum">Minimum allowed.</param>
        /// <param name="maximum">Maximum allowed.</param>
        /// <returns>Coerced number.</returns>
        public static short Coerce(this short source, short minimum, short maximum) {
            if (source < minimum)
                return minimum;
            if (source > maximum)
                return maximum;
            return source;
        }

        /// <summary>
        /// Used to coerce a byte into a range.
        /// </summary>
        /// <param name="source">Number to coerce.</param>
        /// <param name="minimum">Minimum allowed.</param>
        /// <param name="maximum">Maximum allowed.</param>
        /// <returns>Coerced number.</returns>
        public static byte Coerce(this byte source, byte minimum, byte maximum) {
            if (source < minimum)
                return minimum;
            if (source > maximum)
                return maximum;
            return source;
        }

        /// <summary>
        /// Used to coerce a long into a range.
        /// </summary>
        /// <param name="source">Number to coerce.</param>
        /// <param name="minimum">Minimum allowed.</param>
        /// <param name="maximum">Maximum allowed.</param>
        /// <returns>Coerced number.</returns>
        public static long Coerce(this long source, long minimum, long maximum) {
            if (source < minimum)
                return minimum;
            if (source > maximum)
                return maximum;
            return source;
        }
    }
}