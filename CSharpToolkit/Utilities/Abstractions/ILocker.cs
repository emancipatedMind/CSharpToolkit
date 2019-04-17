namespace CSharpToolkit.Utilities.Abstractions {
    using EventArgs;
    using System;
    /// <summary>
    /// Implemented by a class who can provide lock status services.
    /// </summary>
    public interface ILocker : ILockStatusProvider {
        /// <summary>
        /// Used to request a lock.
        /// </summary>
        /// <param name="token">The lock token.</param>
        void RequestLock(object token);
        /// <summary>
        /// Used to request an unlock.
        /// </summary>
        /// <param name="token">The unlock token.</param>
        void RequestUnlock(object token);
    }
}