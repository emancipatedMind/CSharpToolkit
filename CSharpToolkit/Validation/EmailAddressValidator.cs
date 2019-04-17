namespace CSharpToolkit.Validation {
    using Abstractions;
    using System.Text.RegularExpressions;
    using Utilities;
    using System;

    public class EmailAddressValidator : IValidate<string> {

        Regex _emailRegex = Get.EmailRegex();

        public OperationResult Validate(string order) {
            if (order == null)
                return new OperationResult(new[] { new ValidationFailedException("Value was null.") });

            return _emailRegex.IsMatch(order) ?
                new OperationResult() :
                new OperationResult(new[] { new ValidationFailedException("Field is not valid email.") });
        }

    }
}