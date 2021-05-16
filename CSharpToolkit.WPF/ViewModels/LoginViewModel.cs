namespace CSharpToolkit.Login {
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Utilities;
    using Utilities.EventArgs;
    using XAML;
    using XAML.Abstractions;
    using Extensions;
    using Validation;
    using ViewModels.Abstractions;
    using Abstractions;

    public class LoginViewModel<TUser, TRole> : DialogViewModel<GenericEventArgs<Tuple<TUser, List<TRole>>>>, IBindablePassword {

        Locker _disableLocker = new Locker();
        string _userName = "";
        string _errorMessage = "";
        string _domain = "";
        SecureString _password = new SecureString();
        ILoginAppInteractor<TUser, TRole, SecureString> _interactor;

        public LoginViewModel(ILoginAppInteractor<TUser, TRole, SecureString> interactor) {
            _interactor = interactor;
            _disableLocker.LockStatusChanged += (s, e) => OnPropertyChanged(nameof(Disable));

            SuccessEnableCallback = defaultEnable =>
                _interactor.LoginAllowed(UserName, _password) && _disableLocker.IsFree();
            CancelEnableCallback = defaultEnable =>
                _disableLocker.IsFree();
        }

        public AwaitableDelegateCommand Login => Success;
        public ObservableCollection<string> Domains { get; } = new ObservableCollection<string>();
        public SecureString Password {
            set {
                if (Perform.ReplaceIfDifferent(ref _password, value).WasSuccessful) {
                    OnPropertyChanged();
                    ErrorMessage = "";
                }
            }
        }

        public string Domain {
            get { return _domain; }
            set {
                if (Perform.ReplaceIfDifferent(ref _domain, value).WasSuccessful) {
                    OnPropertyChanged();
                    ErrorMessage = "";
                }
            }
        }

        public string UserName {
            get { return _userName; }
            set {
                if (Perform.ReplaceIfDifferent(ref _userName, value).WasSuccessful) {
                    OnPropertyChanged();
                    ErrorMessage = "";
                }
            }
        }

        public string ErrorMessage {
            get { return _errorMessage; }
            set { FirePropertyChangedIfDifferent(ref _errorMessage, value); }
        }

        public bool Disable => _disableLocker.IsLocked();

        public async Task RefreshCollections() {
            OperationResult<string[]> domains =
                await _interactor.GetDomainsAsync();

            if (domains.WasSuccessful) {
                await Perform.CollectionRehydrationAsync(Domains, domains.Result);
                Domain = Domains.FirstOrDefault();
            }
        }

        protected override Task OnSuccessExecuted() =>
            _disableLocker.LockForDurationAsync(async () => {
                OperationResult<Tuple<TUser, List<TRole>>> operation =
                    await _interactor.AttemptLoginAsync(Domain, UserName, _password);

                if (operation.HadErrors) {
                    if (operation.Exceptions.OfType<ValidationFailedException>().Any())
                        ErrorMessage = operation.Exceptions.OfType<ValidationFailedException>().First().Message;
                    else
                        ErrorMessage = "Database operation has failed.";
                    return;
                }

                OnSuccessful(new GenericEventArgs<Tuple<TUser, List<TRole>>>(operation.Result));
            });

        protected override GenericEventArgs<Tuple<TUser, List<TRole>>> GetSuccessObject() =>
            new GenericEventArgs<Tuple<TUser, List<TRole>>>(new Tuple<TUser, List<TRole>>(default(TUser), new List<TRole>()));

    }
}