namespace CSharpToolkit.Validation {
    using Abstractions;
    using System.Text.RegularExpressions;
    using Utilities;
    /// <summary>
    /// Used to verify that characters hostile to sql queries do not exist in a string. By default,
    /// allows alphanumeric characters, escaped single quotes, commas, commercial at(@), and periods. If AllowSingleQuotes is true, single quotes are allowed.
    /// </summary>
    public class SQLInjectionValidator : IValidate<string> {

        Regex _validator;
        Regex _disallowSingleQuotes = new Regex(@"^([a-zA-Z0-9@., ]|(\r)|(\n)|('{2}))*$");
        Regex _allowSingleQuotes = new Regex(@"^([a-zA-Z0-9'@., ]|(\r)|(\n))*$");
        
        /// <summary>
        /// Instantiates SQLInjectionValidator. Used to verify that characters hostile to sql queries do not exist in a string. By default,
        /// allows alphanumeric characters, escaped single quotes, commas, commercial at(@), and periods. If AllowSingleQuotes is true, single quotes are allowed.
        /// </summary>
        public SQLInjectionValidator() {
            AllowSingleQuotes = true;
        } 

        /// <summary>
        /// Validates string to be sure it does not contain characters known to be hostile to sql queries.
        /// </summary>
        /// <param name="order">Statement to validate.</param>
        /// <returns>Operation result object.</returns>
        public OperationResult Validate(string order) =>
            Get.OperationResult(() => {
                if (string.IsNullOrEmpty(order) || _validator.IsMatch(order)) {
                    return true;
                }
                string message =
                    "This field can only contain the following: white space, comma, period, a-z, A-Z, 0-9, @, and";

                if (AllowSingleQuotes)
                    message += " a single quote.";
                else
                    message += " a single quote not followed by another single quote.";

                throw new ValidationFailedException(message);
            });

        /// <summary>
        /// Whether single quotes are allowed or not.
        /// </summary>
        public bool AllowSingleQuotes {
            get { return _validator.Equals(_allowSingleQuotes); }
            set { _validator = value ? _allowSingleQuotes : _disallowSingleQuotes; }
        }

    }
}