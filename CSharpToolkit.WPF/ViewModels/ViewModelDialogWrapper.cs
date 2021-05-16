namespace CSharpToolkit.ViewModels {
    using System;
    using System.Windows.Input;
    using System.Threading.Tasks;
    using CSharpToolkit.XAML;
    using CSharpToolkit.XAML.Abstractions;
    using CSharpToolkit.Utilities.Abstractions;
    using CSharpToolkit.Utilities.EventArgs;
    using Utilities;
    using Utilities.Abstractions;

    public class ViewModelDialogWrapper<TViewModel> : DialogEntityBase<GenericEventArgs<TViewModel>>, IViewModelStateProvider, IParentViewModel where TViewModel : IViewModelStateProvider, IParentViewModel, IDisposable {

        private bool _disposedValue = false;

        public ViewModelDialogWrapper(TViewModel viewModel) {
            ViewModel = viewModel;
        } 

        public TViewModel ViewModel { get; }
        public ViewModelState State => ViewModel.State;

        public ILocker GetFamilialLocker() => ViewModel.GetFamilialLocker();
        public ILocker GetFormRestrictor() => ViewModel.GetFormRestrictor();

        protected override bool DefaultSuccessEnableCallback() =>
            HasErrors == false && State != ViewModelState.FamilialOperation;

        protected override void Dispose(bool disposing) {
            if (!_disposedValue) {
                if (disposing) { }
                _disposedValue = true;
            }
            base.Dispose(disposing);
        }

        protected override GenericEventArgs<TViewModel> GetSuccessObject() =>
            new GenericEventArgs<TViewModel>(ViewModel);
    }
}
