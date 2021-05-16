namespace CSharpToolkit.ViewModels {
    using System.Reflection;
    using Abstractions;
    using CSharpToolkit.Extensions;
    using CSharpToolkit.Utilities.EventArgs;
    using CSharpToolkit.XAML;
    using CSharpToolkit.ViewModels.Abstractions;

    public class ModifyingRecordViewModel<TViewModel> : DialogViewModel<GenericEventArgs<TViewModel>> where TViewModel : EntityBase {

        static bool IsValidationBase = false;
        static PropertyInfo ModifyingProperty;

        static ModifyingRecordViewModel() {
            IsValidationBase = typeof(TViewModel).IsSubClassOfGeneric(typeof(ValidationBase));
            if (IsValidationBase) {
                ModifyingProperty =
                    typeof(TViewModel)
                        .GetProperty(nameof(ValidationBase.Modifying), BindingFlags.Instance | BindingFlags.Public);
            } 
        }

        private bool _disposedValue;
        TViewModel _viewModel;

        public ModifyingRecordViewModel(TViewModel viewModel) {
            Field = viewModel;
            if (IsValidationBase) {
                ModifyingProperty?.SetValue(viewModel, true);
            }
            Field.ErrorsChanged += ViewModel_ErrorsChanged;
        }

        private void ViewModel_ErrorsChanged(object sender, System.ComponentModel.DataErrorsChangedEventArgs e) {
            if (Field.HasErrors) {
                var errorsEnumerator = Field.GetErrors("").GetEnumerator();
                errorsEnumerator.MoveNext();
                AddError(nameof(Field), errorsEnumerator.Current.ToString());
            }
            else ClearErrors(nameof(Field));
        }

        public TViewModel Field {
            get { return _viewModel; }
            private set { FirePropertyChangedIfDifferent(ref _viewModel, value); }
        }

        protected override void Dispose(bool disposing) {
            if (!_disposedValue) {
                if (disposing) {
                    Field.ErrorsChanged -= ViewModel_ErrorsChanged;
                }
                _disposedValue = true;
            }
            base.Dispose(disposing);
        }

        protected override GenericEventArgs<TViewModel> GetSuccessObject() =>
            new GenericEventArgs<TViewModel>(Field);
    }
}
