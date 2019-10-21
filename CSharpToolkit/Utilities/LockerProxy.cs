namespace CSharpToolkit.Utilities {
    using System;
    using Abstractions;
    using EventArgs;
    using System.Collections.Generic;

    public class LockerProxy : ILocker {
        static Func<LockStatus> DefaultCurrentStatusCallback = new Func<LockStatus>(() => LockStatus.Free);

        bool _disposed = false;
        Func<LockStatus> _currentStatusCallback;
        List<object> _tokens = new List<object>();

        public LockStatus CurrentStatus => CurrentStatusCallback();

        public event EventHandler<GenericEventArgs<LockStatus>> LockStatusChanged;
        public event EventHandler<GenericEventArgs<object>> LockRequested;
        public event EventHandler<GenericEventArgs<object>> UnLockRequested;

        public void RequestLock(object token) {
            if (_disposed)
                return;
            _tokens.Add(token);
            LockRequested?.Invoke(this, new GenericEventArgs<object>(token));
        }

        public void RequestUnlock(object token) {
            if (_disposed)
                return;
            UnLockRequested?.Invoke(this, new GenericEventArgs<object>(token));
            _tokens.Remove(token);
        }

        public Func<LockStatus> CurrentStatusCallback {
            get { return _currentStatusCallback ?? DefaultCurrentStatusCallback; }
            set { _currentStatusCallback = value; }
        }

        public void FireLockStatusChanged(LockStatus status) =>
            LockStatusChanged?.Invoke(this, new GenericEventArgs<LockStatus>(status));

        #region IDisposable Support
        protected virtual void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    _tokens.ForEach(token => UnLockRequested?.Invoke(this, new GenericEventArgs<object>(token)));
                    UnLockRequested = null;
                    LockStatusChanged = null;
                    LockRequested = null;
                    _currentStatusCallback = null;
                    _tokens.Clear();
                    _tokens = null;
                }
                _disposed = true;
            }
        }

        public void Dispose() => Dispose(true);
        #endregion

    }
}