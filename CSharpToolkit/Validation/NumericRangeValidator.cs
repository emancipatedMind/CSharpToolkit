namespace CSharpToolkit.Validation {
    using Utilities;
    /// <summary>
    /// Used to verify that field is not outside of range.
    /// </summary>
    public class NumericRangeValidator : Abstractions.IValidate<RangeDataOrder<long>> {

        /// <summary>
        /// Instantiates NumericRangeValidator. Used to validate number.
        /// </summary>
        public NumericRangeValidator() { }

        /// <summary>
        /// Validates number to assure number is inside of range.
        /// </summary>
        /// <param name="order">Number to validate.</param>
        /// <returns>Operation result object.</returns>
        public OperationResult Validate(RangeDataOrder<long> order) =>
            Get.OperationResult(() => {
                if ((order.Field < order.Minimum) || (order.Field > order.Maximum))
                    throw new ValidationFailedException($"The accepted range is {order.Minimum} to {order.Maximum}.");
            });
    }
}