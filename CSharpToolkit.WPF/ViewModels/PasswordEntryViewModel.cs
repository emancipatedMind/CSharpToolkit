namespace CSharpToolkit.ViewModels {
    using Abstractions;
    using Utilities;
    using Utilities.Abstractions;
    using Utilities.EventArgs;
    using XAML;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    public class PasswordEntryViewModel : DialogViewModel<GenericEventArgs<OperationResult>> {
        IConverter<SecureString, Task<OperationResult>> _verifyPassword;

        public PasswordEntryViewModel(IConverter<SecureString, Task<OperationResult>> verifyPassword) {
            _verifyPassword = verifyPassword;
        }

        protected override async Task OnSuccessExecuted() {
            OperationResult ValidPassword = await _verifyPassword.Convert(Password.Password);

            if (ValidPassword.WasSuccessful == false) {
                if (ValidPassword.HadErrors) {
                    OnCancelled();
                    return;
                }

                InvalidPasswordEntered?.Invoke(this, EventArgs.Empty);
                return;
            }

            OnSuccessful(new GenericEventArgs<OperationResult>(ValidPassword));
            Dispose();
            return;
        }

        protected override GenericEventArgs<OperationResult> GetSuccessObject() {
            throw new NotImplementedException();
        }

        public event EventHandler InvalidPasswordEntered;

        public BindablePasswordViewModel Password { get; } = new BindablePasswordViewModel();
    }
}
