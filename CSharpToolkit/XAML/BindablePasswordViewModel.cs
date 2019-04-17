namespace CSharpToolkit.XAML {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using Abstractions;
    using Utilities;
    using Validation;
    using Validation.Abstractions;
    public class BindablePasswordViewModel : EntityBase, IBindablePassword {
        SecureString _password = new SecureString();
        IValidate<SecureString> _validator;

        public SecureString Password {
            get { return _password; }
            set {
                if (Perform.ReplaceIfDifferent(ref _password, value).WasSuccessful) {
                    OnPropertyChanged();
                    RunValidation();
                } 
            }
        }

        public IValidate<SecureString> Validator {
            get { return _validator ?? NullValidator<SecureString>.Instance; }
            set { _validator = value; }
        }

        public void RunValidation() {
            ClearErrors(nameof(Password));
            AddErrors(nameof(Password), Validator.Validate(Password).Exceptions.Select(e => e.Message).Distinct());
        }

    }
}