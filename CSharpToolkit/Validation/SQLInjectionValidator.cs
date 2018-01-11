namespace CSharpToolkit.Validation {
    using Abstractions;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using Utilities;
    /// <summary>
    /// Used to verify that invalid characters do not exist in string collection. Only allows alphanumeric characters,
    /// and escaped single quotes. If AllowSingleQuotes is true, single quotes are allowed.
    /// </summary>
    public class SQLInjectionValidator : IValidate<IEnumerable<string>> {
        
        RegexStringValidator _disallowSingleQuotes = new RegexStringValidator(@"^([a-zA-Z0-9]|('{2}))*$");
        RegexStringValidator _allowSingleQuotes = new RegexStringValidator(@"^([a-zA-Z0-9'])*$");

        RegexStringValidator _validator;

        /// <summary>
        /// Instantiates SQLInjectionValidator.
        /// </summary>
        public SQLInjectionValidator() {
            _validator = _disallowSingleQuotes;
        } 

        /// <summary>
        /// Validates collection of strings for any invalid characters. 
        /// </summary>
        /// <param name="order">Statements to validate.</param>
        /// <returns>Operation result object.</returns>
        public OperationResult Validate(IEnumerable<string> order) =>
            Get.OperationResult(() => {
                order.ToList().ForEach(t => _validator.Validate(t)); 
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