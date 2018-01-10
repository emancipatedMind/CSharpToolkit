namespace CSharpToolkit.Validation {
    using Abstractions;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using Utilities;
    public class SQLInjectionValidator : IValidate<IEnumerable<string>> {

        
        RegexStringValidator _disallowSingleQuotes = new RegexStringValidator(@"^([a-zA-Z0-9]|('{2}))*$");
        RegexStringValidator _allowSingleQuotes = new RegexStringValidator(@"^([a-zA-Z0-9'])*$");

        RegexStringValidator _validator;

        public SQLInjectionValidator() {
            _validator = _disallowSingleQuotes;
        } 

        public OperationResult Validate(IEnumerable<string> order) =>
            Get.OperationResult(() => {
                order.ToList().ForEach(t => _validator.Validate(t)); 
            });

        public bool AllowSingleQuotes {
            get { return _validator.Equals(_allowSingleQuotes); }
            set { _validator = value ? _allowSingleQuotes : _disallowSingleQuotes; }
        }

    }
}