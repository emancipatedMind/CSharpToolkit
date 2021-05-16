namespace CSharpToolkit.Utilities {
    using System;
    using System.Collections.Generic;
    using Abstractions;
    using EventArgs;
    using Extensions;
    using System.Linq;
    public class AggregatedLockers : ILocker {

        private bool _disposed = false;
        List<ILocker> _lockers = new List<ILocker>();
        List<object> _tokens = new List<object>();
        bool _idle = true;
        LockStatus storedStatus = LockStatus.Free;

        public LockStatus CurrentStatus => _lockers.Any(locker => locker.CurrentStatus == LockStatus.Locked) ? LockStatus.Locked : LockStatus.Free;

        public event EventHandler<GenericEventArgs<LockStatus>> LockStatusChanged;

        public void RequestLock(object token) =>
            OperationWrapper(() => {
                _lockers.ForEach(locker => {
                    _tokens.Add(token);
                    locker.RequestLock(token);
                });
            });

        public void RequestUnlock(object token) =>
            OperationWrapper(() => {
                _lockers.ForEach(locker => {
                    _tokens.Remove(token);
                    locker.RequestUnlock(token);
                });
            });

        public void AddRange(IEnumerable<ILocker> lockers) => lockers.ForEach(Add);
        public void Add(ILocker locker) =>
            OperationWrapper(() => {
                if (_lockers.Contains(locker))
                    return;
                locker.LockStatusChanged += Locker_LockStatusChanged;
                _lockers.Add(locker);
            });

        public void Remove(ILocker locker) =>
            OperationWrapper(() => {
                locker.LockStatusChanged -= Locker_LockStatusChanged;
                _lockers.Remove(locker);
            });

        private void Locker_LockStatusChanged(object sender, GenericEventArgs<LockStatus> e) {
            if (_idle && storedStatus != CurrentStatus) {
                storedStatus = CurrentStatus;
                LockStatusChanged?.Invoke(this, new GenericEventArgs<LockStatus>(CurrentStatus));
            }
        }

        void OperationWrapper(Action callback) {
            _idle = false;
            var preOperationStatus = CurrentStatus;
            callback();
            if (CurrentStatus != preOperationStatus)
                LockStatusChanged?.Invoke(this, new GenericEventArgs<LockStatus>(CurrentStatus));
            _idle = true;
        }

        #region IDisposable Support
        protected virtual void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    _lockers.ForEach(locker =>
                        locker.LockStatusChanged -= Locker_LockStatusChanged
                    );
                    _lockers.ForEach(locker => 
                        _tokens.ForEach(token => locker.RequestUnlock(token))
                    );
                    _tokens.Clear();
                    _lockers.Clear();
                    _lockers = null;
                    _tokens = null;
                }
                _disposed = true;
            }
        }

        public void Dispose() => Dispose(true);
        #endregion

    }
}