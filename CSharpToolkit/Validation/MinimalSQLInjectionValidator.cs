namespace CSharpToolkit.Validation {
    using Abstractions;
    using System.Text.RegularExpressions;
    using Utilities;
    /// <summary>
    /// Used to verify that characters hostile to sql queries do not exist in a string. By default,
    /// disallows two consecutive dashes, a semi-colon, 'xp_', and '/* */'.
    /// </summary>
    public class MinimalSQLInjectionValidator : IValidate<string> {

        Regex _validator = new Regex(@"(-{2})|(;)|(xp_)|(/\*.*\*/)");

        /// <summary>
        /// Instantiates SQLInjectionValidator. Used to verify that characters hostile to sql queries do not exist in a string. By default,
        /// disallows two consecutive dashes, a semi-colon, 'xp_', and '/* */'.
        /// </summary>
        public MinimalSQLInjectionValidator() { }

        /// <summary>
        /// Validates string to be sure it does not contain characters known to be hostile to sql queries.
        /// </summary>
        /// <param name="order">Statement to validate.</param>
        /// <returns>Operation result object.</returns>
        public OperationResult Validate(string order) =>
            Get.OperationResult(() => {
                if (string.IsNullOrEmpty(order) == false && _validator.IsMatch(order)) {
                    throw new ValidationFailedException("This field disallows the following: two consecutive dashes, a semi-colon, 'xp_', and '/* */'.");
                }
            });

    }
}