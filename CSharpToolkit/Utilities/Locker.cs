namespace CSharpToolkit.Utilities {
    using Abstractions;
    using EventArgs;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    /// <summary>
    /// Used to provide lock services.
    /// </summary>
    public class Locker : ILocker {

        List<object> _tokens = new List<object>();

        /// <summary>
        /// The current status of the locker.
        /// </summary>
        public LockStatus CurrentStatus => _tokens.Count == 0 ? LockStatus.Free : LockStatus.Locked;

        /// <summary>
        /// An event that signals the lock status has changed.
        /// </summary>
        public event EventHandler<GenericEventArgs<LockStatus>> LockStatusChanged;

        /// <summary>
        /// Used to request a lock.
        /// </summary>
        /// <param name="token">The lock token.</param>
        public void RequestLock(object token) {
            if (token == null)
                return;

            bool fireEvent = _tokens.Count == 0;

            if (_tokens.Contains(token) == false)
                _tokens.Add(token);

            if (fireEvent)
                LockStatusChanged?.Invoke(this, new GenericEventArgs<LockStatus>(LockStatus.Locked));
        }

        /// <summary>
        /// Used to request an unlock.
        /// </summary>
        /// <param name="token">The token used when the lock was requested.</param>
        public void RequestUnlock(object token) {
            bool fireEvent = _tokens.Remove(token) && _tokens.Any() == false;
            if (fireEvent)
                LockStatusChanged?.Invoke(this, new GenericEventArgs<LockStatus>(LockStatus.Free));
        }

    }
}