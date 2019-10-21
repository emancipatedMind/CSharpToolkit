namespace CSharpToolkit.Utilities {
    using System;
    using Abstractions;
    using EventArgs;

    /// <summary>
    /// This decorator marshals all requests to <see cref="Component"/>. The functionality given is that this decorator is always safe to call, and gives caller one instance to call.
    /// </summary>
    public class SafeReferenceLockerDecorator : ILocker {

        Locker DefaultLocker = new Locker();
        bool _disposed = false; // To detect redundant calls
        ILocker _component;

        /// <summary>
        /// Instantiates the <see cref="SafeReferenceLockerDecorator"/> class.
        /// </summary>
        public SafeReferenceLockerDecorator() {
            Component = DefaultLocker;
        }

        /// <summary>
        /// The <see cref="CurrentStatus"/> of the marshaled <see cref="Component"/>.
        /// </summary>
        public LockStatus CurrentStatus => _component?.CurrentStatus ?? LockStatus.Free;

        /// <summary>
        /// Linked to the <see cref="LockStatusChanged"/> of the <see cref="Component"/>.
        /// </summary>
        public event EventHandler<GenericEventArgs<LockStatus>> LockStatusChanged;

        /// <summary>
        /// The object through which all requests are marshaled.
        /// </summary>
        public ILocker Component {
            get { return _component; }
            set {
                var oldValue = _component;
                if (Perform.ReplaceIfDifferent(ref _component, value, DefaultLocker).WasSuccessful) {
                    SwapComponent(_component, oldValue);
                }
            }
        }

        private void SwapComponent(ILocker newComponent, ILocker oldComponent) {
            if (newComponent != null)
                newComponent.LockStatusChanged += Component_LockStatusChanged;
            if (oldComponent != null)
                oldComponent.LockStatusChanged -= Component_LockStatusChanged;
        }

        private void Component_LockStatusChanged(object sender, GenericEventArgs<LockStatus> e) =>
            LockStatusChanged?.Invoke(sender, e);

        /// <summary>
        /// Passes request to inner <see cref="Component"/>.
        /// </summary>
        /// <param name="token">The lock token.</param>
        public void RequestLock(object token) => _component?.RequestLock(token);

        /// <summary>
        /// Passes request to inner <see cref="Component"/>.
        /// </summary>
        /// <param name="token">The unlock token.</param>
        public void RequestUnlock(object token) => _component?.RequestUnlock(token);

        #region IDisposable Support
        protected virtual void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    Component = null;
                    DefaultLocker.Dispose();
                    LockStatusChanged?.Invoke(this, new GenericEventArgs<LockStatus>(LockStatus.Free));
                    LockStatusChanged = null;
                }
                _disposed = true;
            }
        }

        public void Dispose() => Dispose(true);
        #endregion

    }
}