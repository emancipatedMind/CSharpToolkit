namespace CSharpToolkit.XAML {
    using System;
    using System.Threading.Tasks;
    using Abstractions;
    using System.Windows.Input;
    using Utilities;

    public abstract class DialogEntityBase<TEventArgs> : EntityBase, IDialogControl<TEventArgs>, IDisposable where TEventArgs : EventArgs {

        static Func<bool, bool> DefaultEnable = b => b;
        static Func<Task<OperationResult>> DefaultSuccessValidationCallback = () => Task.FromResult(new OperationResult());

        private Func<Task<OperationResult>> _successValidationCallback;
        private Func<bool, bool> _cancelEnableCallback;
        private Func<bool, bool> _successEnableCallback;
        private bool _disposedValue = false;

        public DialogEntityBase() {
            Success = new AwaitableDelegateCommand(SuccessCallback, SuccessEnable);
            Cancel = new AwaitableDelegateCommand(CancelCallback, CancelEnable);
        }

        public AwaitableDelegateCommand Success { get; }
        public AwaitableDelegateCommand Cancel { get; }
        ICommand IDialogControl<TEventArgs>.Cancel => Cancel;

        public bool DisposeOnSuccess { get; set; }
        public bool DisposeOnCancel { get; set; } = true;

        public event EventHandler Cancelled;
        public event EventHandler<TEventArgs> Successful;

        public Func<Task<OperationResult>> SuccessValidationCallback {
            get { return _successValidationCallback ?? DefaultSuccessValidationCallback; }
            set { _successValidationCallback = value; }
        }

        public Func<bool, bool> SuccessEnableCallback {
            get { return _successEnableCallback ?? DefaultEnable; }
            set { _successEnableCallback = value; }
        }

        public Func<bool, bool> CancelEnableCallback {
            get { return _cancelEnableCallback ?? DefaultEnable; }
            set { _cancelEnableCallback = value; }
        }

        protected virtual async Task OnSuccessExecuted() {
            var successValiationCallback =
                await SuccessValidationCallback();
            if (successValiationCallback.WasSuccessful == false) {
                if (successValiationCallback.HadErrors)
                    await OnCancelExecuted();
                return;
            }

            OnSuccessful();
            if (DisposeOnSuccess)
                Dispose();
        }
        protected virtual Task OnCancelExecuted() {
            OnCancelled();
            if (DisposeOnCancel)
                Dispose();
            return Task.CompletedTask;
        }
        protected abstract TEventArgs GetSuccessObject();
        protected void OnSuccessful() => OnSuccessful(GetSuccessObject());
        protected void OnSuccessful(TEventArgs arg) => Successful?.Invoke(this, arg);
        protected void OnCancelled() => Cancelled?.Invoke(this, EventArgs.Empty);

        private async Task SuccessCallback() => await OnSuccessExecuted();
        private bool SuccessEnable() => SuccessEnableCallback(DefaultSuccessEnableCallback());
        private async Task CancelCallback() => await OnCancelExecuted();
        private bool CancelEnable() => CancelEnableCallback(DefaultCancelEnableCallback());

        protected virtual bool DefaultSuccessEnableCallback() =>
            HasErrors == false;
        protected virtual bool DefaultCancelEnableCallback() =>
            true;

        protected override void Dispose(bool disposing) {
            if (!_disposedValue) {
                if (disposing) {
                    Successful = null;
                    Cancelled = null;
                    _cancelEnableCallback = null;
                    _successEnableCallback = null;
                    _successValidationCallback = null;
                }
                _disposedValue = true;
            }
            base.Dispose(disposing);
        }

    }
}
