namespace CSharpToolkit.XAML {
    using Abstractions;
    using System;
    using System.Windows;
    public class DialogSignalerEnvironment<T> : IDisposable, IDialogSignaler<T> where T : EventArgs {

        public DialogSignalerEnvironment(IDialogSignaler<T> dialogController, Window dialog) {
            Dialog = dialog;
            Dialog.Closed += OnDialogClosed;
            DialogController = dialogController;
            DialogController.Cancelled += OnCancelled;
            DialogController.Successful += OnSuccessful;
        }

        public Window Dialog { get; private set; }
        public IDialogSignaler<T> DialogController { get; private set; }

        public event EventHandler DialogClosed;
        public event EventHandler Cancelled;
        public event EventHandler<T> Successful;

        void OnDialogClosed(object sender, EventArgs e) {
            DialogClosed?.Invoke(this, e);
        } 

        void OnCancelled(object sender, EventArgs e) {
            Cancelled?.Invoke(this, e);
        }

        void OnSuccessful(object sender, T e) {
            Successful?.Invoke(this, e);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    Dialog.Closed -= OnDialogClosed;
                    DialogController.Cancelled -= OnCancelled;
                    DialogController.Successful -= OnSuccessful;
                    Dialog = null;
                    DialogController = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DialogSignalerEnvironment() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose() {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}