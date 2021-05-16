namespace CSharpToolkit.Validation {
    using Utilities;
    /// <summary>
    /// Checks to assure that a required field is filled in.
    /// </summary>
    public class RequiredValidator : Abstractions.IValidate<string> {

        /// <summary>
        /// Checks to assure that a required field contains information.
        /// </summary>
        public RequiredValidator() { }

        /// <summary>
        /// Validates string to be sure it does indeed contain information.
        /// </summary>
        /// <param name="order">Statement to validate.</param>
        /// <returns>Operation result object.</returns>
        public OperationResult Validate(string order) =>
            Get.OperationResult(() => {
                if (order == null || string.IsNullOrWhiteSpace(order))
                    throw new ValidationFailedException("Field is required.");
            });
    }
}