namespace CSharpToolkit.ViewModels {
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Abstractions;
    using CSharpToolkit.Utilities;
    using Utilities.Abstractions;
    using CSharpToolkit.XAML;
    public class LocatorViewModel<TLocator> : DialogViewModel<EventArgs> {

        IFormInteractor _interactor;

        public LocatorViewModel(TLocator field, IFormInteractor interactor) {
            Locator = field;
            _interactor = interactor;

            SideBarCommands.OpenMainWindow = new AwaitableDelegateCommand(_interactor.ActivateMainWindow);
            SideBarCommands.DeleteAvailable = false;
        }

        TLocator _field;

        public CSharpToolkit.Validation.Abstractions.IValidate<TLocator> Validator { get; set; }
        public TLocator Locator {
            get { return _field; }
            set {
                if (Perform.ReplaceIfDifferent(ref _field, value).WasSuccessful) {
                    OnPropertyChanged();
                }
            }
        }

        public override string this[string columnName] {
            get {
                ClearErrors(columnName);

                if (Validator != null && columnName == nameof(Locator)) {
                    OperationResult operation = Validator.Validate(Locator);

                    if (operation.HadErrors)
                        AddErrors(columnName, operation.Exceptions.Select(ex => ex.Message));
                }

                return "";
            }
        }

        public AdminSideBarButtonsViewModel SideBarCommands { get; } = new AdminSideBarButtonsViewModel();

        protected override EventArgs GetSuccessObject() => EventArgs.Empty;

    }
}
