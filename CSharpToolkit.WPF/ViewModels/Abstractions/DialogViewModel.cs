namespace CSharpToolkit.ViewModels.Abstractions {
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Utilities;
    using XAML;
    using XAML.Abstractions;
    public abstract class DialogViewModel<TEventArgs> : WindowViewModel, IDialogControl<TEventArgs>, IDisposable where TEventArgs : EventArgs {

        static Func<bool, bool> DefaultEnable = b => b;

        private Func<bool, bool> _cancelEnableCallback;
        private Func<bool, bool> _successEnableCallback;
        private bool _disposedValue = false;

        public DialogViewModel() {
            Success = new AwaitableDelegateCommand(SuccessCallback, SuccessEnable);
            Cancel = new AwaitableDelegateCommand(CancelCallback, CancelEnable);
        }

        public event EventHandler Cancelled;
        public event EventHandler<TEventArgs> Successful;

        public AwaitableDelegateCommand Success { get; }
        public AwaitableDelegateCommand Cancel { get; }

        public Func<bool, bool> SuccessEnableCallback {
            get { return _successEnableCallback ?? DefaultEnable; }
            set { _successEnableCallback = value; }
        }

        public Func<bool, bool> CancelEnableCallback {
            get { return _cancelEnableCallback ?? DefaultEnable; }
            set { _cancelEnableCallback = value; }
        }

        public bool DisposeOnCancel { get; set; } = true;
        public bool DisposeOnSuccess { get; set; } = false;

        ICommand IDialogControl<TEventArgs>.Cancel => Cancel;

        protected abstract TEventArgs GetSuccessObject();

        protected virtual Task OnSuccessExecuted() {
            OnSuccessful(GetSuccessObject());
            return Task.CompletedTask;
        }

        protected virtual Task OnCancelExecuted() {
            Cancelled?.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        }

        protected void OnSuccessful(TEventArgs args) => Successful?.Invoke(this, args);
        protected void OnCancelled() => Cancelled?.Invoke(this, EventArgs.Empty);

        private async Task SuccessCallback() {
            await OnSuccessExecuted();
            if (DisposeOnSuccess)
                Dispose(true);
        }
        private async Task CancelCallback() {
            await OnCancelExecuted();
            if (DisposeOnCancel)
                Dispose(true);
        }

        private bool SuccessEnable() => SuccessEnableCallback(DefaultSuccessEnableCallback());
        private bool CancelEnable() => CancelEnableCallback(DefaultCancelEnableCallback());

        private bool DefaultSuccessEnableCallback() =>
            HasErrors == false &&
                State != ViewModelState.FamilialOperation;
        private bool DefaultCancelEnableCallback() =>
            true;

        protected override void Dispose(bool disposing) {
            if (!_disposedValue) {
                if (disposing) {
                    Successful = null;
                    Cancelled = null;
                }
                _disposedValue = true;
            }
            base.Dispose(disposing);
        }

    }
}
