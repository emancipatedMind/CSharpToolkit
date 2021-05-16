namespace CSharpToolkit.Validation {

    /// <summary>
    /// Data order used for validating a range.
    /// </summary>
    /// <typeparam name="T">Field type.</typeparam>
    public struct RangeDataOrder<T> {

        /// <summary>
        /// Data order that contains order information for a range.
        /// </summary>
        /// <param name="field">Field to be validated.</param>
        /// <param name="minimum">Maximum allowed by field.</param>
        /// <param name="maximum">Minimum allowed by field.</param>
        public RangeDataOrder(T field, T minimum, T maximum) {
            Field = field;
            Minimum = minimum;
            Maximum = maximum;
        }

        /// <summary>
        /// Field to be validated.
        /// </summary>
        public T Field { get; }

        /// <summary>
        /// Maximum allowed by field.
        /// </summary>
        public T Maximum { get; }

        /// <summary>
        /// Minimum allowed by field.
        /// </summary>
        public T Minimum { get; }

    }
}