namespace Schedule.Abstractions {
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using CSharpToolkit.XAML;
    using CSharpToolkit.XAML.Abstractions;

    public abstract class DialogViewModel<T> : EntityBase, IDialogControl<T> where T : EventArgs {

        private bool _disposedValue;

        public DialogViewModel() {
            Success = new AwaitableDelegateCommand(SuccessCallback, SuccessEnable);
            Cancel = new AwaitableDelegateCommand(CancelCallback);
        }

        public AwaitableDelegateCommand Cancel { get; }
        public AwaitableDelegateCommand Success { get; }

        ICommand IDialogControl<T>.Cancel => Cancel;

        public event EventHandler Cancelled;
        public event EventHandler<T> Successful;

        Task SuccessCallback() => OnSuccess();
        Task CancelCallback() => OnCancel();

        protected bool SuccessEnable() =>
            HasErrors == false;

        protected virtual Task OnCancel() {
            Cancelled?.Invoke(this, EventArgs.Empty);
            Dispose();
            return Task.CompletedTask;
        }

        protected virtual Task OnSuccess() {
            Successful?.Invoke(this, GetSuccessObject());
            Dispose();
            return Task.CompletedTask;
        }

        protected abstract T GetSuccessObject();

        protected override void Dispose(bool disposing) {
            if (_disposedValue == false) {
                if (disposing) {
                    Successful = null;
                    Cancelled = null;
                }
                _disposedValue = true;
            }
        }

    }
}