namespace CSharpToolkit.Utilities {
    using System;
    using Abstractions;
    using EventArgs;

    public class LockerProxy : ILocker, IDisposable {

        Func<LockStatus> _currentStatusCallback;

        public LockStatus CurrentStatus => CurrentStatusCallback();

        public event EventHandler<GenericEventArgs<LockStatus>> LockStatusChanged;
        public event EventHandler<GenericEventArgs<object>> LockRequested;
        public event EventHandler<GenericEventArgs<object>> UnLockRequested;

        public void RequestLock(object token) =>
            LockRequested?.Invoke(this, new GenericEventArgs<object>(token));

        public void RequestUnlock(object token) =>
            UnLockRequested?.Invoke(this, new GenericEventArgs<object>(token));

        public Func<LockStatus> CurrentStatusCallback {
            get { return _currentStatusCallback ?? new Func<LockStatus>(() => LockStatus.Free); }
            set { _currentStatusCallback = value; }
        }

        public void FireLockStatusChanged(LockStatus status) =>
            LockStatusChanged?.Invoke(this, new GenericEventArgs<LockStatus>(status));

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    // TODO: dispose managed state (managed objects).
                }

                LockStatusChanged = null;
                LockRequested = null;
                UnLockRequested = null;

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~LockerProxy() {
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