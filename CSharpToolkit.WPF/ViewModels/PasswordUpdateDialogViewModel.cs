namespace CSharpToolkit.ViewModels {
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Abstractions;
    using CSharpToolkit.XAML;
    using CSharpToolkit.Validation.Abstractions;
    using CSharpToolkit.Utilities;
    using System;
    using CSharpToolkit.Utilities.EventArgs;
    using CSharpToolkit.Validation;

    public class PasswordUpdateDialogViewModel : DialogViewModel<GenericEventArgs<SecureString>> {

        public PasswordUpdateDialogViewModel() : this(NullValidator<SecureString>.Instance, NullValidator<SecureString>.Instance) { }
        public PasswordUpdateDialogViewModel(IValidate<SecureString> originalValidator) : this(originalValidator, NullValidator<SecureString>.Instance) { }
        public PasswordUpdateDialogViewModel(IValidate<SecureString> originalValidator, IValidate<SecureString> confirmValidator) {
            SuccessEnableCallback = defaultEnable =>
                (Password.HasErrors || ConfirmPassword.HasErrors) == false;

            Password.Validator = originalValidator;
            if (confirmValidator is NullValidator<SecureString>) {
                confirmValidator = new CallbackValidator<SecureString>(o =>
                    Password.Password.Length != 0 && Perform.SecureStringCompare(Password.Password, ConfirmPassword.Password) ?
                        new OperationResult() :
                        new OperationResult(new[] { new ValidationFailedException("Both fields must match.") })
                );
            }
            ConfirmPassword.Validator = confirmValidator;

            Password.PropertyChanged += Password_PropertyChanged;
            ConfirmPassword.PropertyChanged += Password_PropertyChanged;

            Password.RunValidation();
            ConfirmPassword.RunValidation();
        }

        private void Password_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(Password)) {
                if (sender != Password)
                    Password.RunValidation();
                if (sender != ConfirmPassword)
                    ConfirmPassword.RunValidation();
            }
        }

        public BindablePasswordViewModel Password { get; } = new BindablePasswordViewModel();
        public BindablePasswordViewModel ConfirmPassword { get; } = new BindablePasswordViewModel();

        protected override GenericEventArgs<SecureString> GetSuccessObject() =>
            new GenericEventArgs<SecureString>(Password.Password);

    }
}
