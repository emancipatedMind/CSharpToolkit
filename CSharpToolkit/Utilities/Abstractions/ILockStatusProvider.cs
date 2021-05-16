namespace CSharpToolkit.Utilities.Abstractions {
    using EventArgs;
    using System;
    /// <summary>
    /// Implemented by a class who can provide lock status.
    /// </summary>
    public interface ILockStatusProvider {
        /// <summary>
        /// An event that signals the lock status has changed.
        /// </summary>
        event EventHandler<GenericEventArgs<LockStatus>> LockStatusChanged;
        /// <summary>
        /// The current status of the locker.
        /// </summary>
        LockStatus CurrentStatus { get; }
    }
}