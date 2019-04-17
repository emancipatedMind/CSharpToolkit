namespace CSharpToolkit.Utilities {
    using System;
    using System.Collections.Generic;
    using Abstractions;
    using EventArgs;
    using Extensions;
    public class AggregatedLockers : ILocker {

        readonly Locker locker = new Locker();
        readonly List<ILocker> _lockers = new List<ILocker>();

        public LockStatus CurrentStatus => locker.CurrentStatus;

        public event EventHandler<GenericEventArgs<LockStatus>> LockStatusChanged;

        public AggregatedLockers() {
            locker.LockStatusChanged += (s, e) => LockStatusChanged?.Invoke(this, e);
        }

        public void RequestLock(object token) => _lockers.ForEach(locker => locker.RequestLock(token));
        public void RequestUnlock(object token) => _lockers.ForEach(locker => locker.RequestUnlock(token));

        public void Add(ILocker locker) {
            if (_lockers.Contains(locker))
                return;
            locker.LockStatusChanged += Locker_LockStatusChanged;
            _lockers.Add(locker);
            if (locker.CurrentStatus == LockStatus.Locked)
                locker.RequestLock(locker);
        }

        public void AddRange(IEnumerable<ILocker> lockers) => lockers.ForEach(Add);

        public void Remove(ILocker locker) {
            locker.LockStatusChanged -= Locker_LockStatusChanged;
            locker.RequestUnlock(locker);
            _lockers.Remove(locker);
        }

        private void Locker_LockStatusChanged(object sender, GenericEventArgs<LockStatus> e) {
            if (e.Data == LockStatus.Free) {
                locker.RequestUnlock(sender);
            }
            else {
                locker.RequestLock(sender);
            }
        }

    }
}