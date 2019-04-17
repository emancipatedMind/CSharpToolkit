namespace CSharpToolkit.Utilities {
    using System;
    using Abstractions;
    using EventArgs;

    /// <summary>
    /// This decorator marshals all requests to <see cref="Component"/>. The functionality given is that this decorator is always safe to call, and gives caller one instance to call.
    /// </summary>
    public class SafeReferenceLockerDecorator : ILocker, IDisposable {

        Locker DefaultLocker = new Locker();

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
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                Component.LockStatusChanged -= Component_LockStatusChanged;

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SafeReferenceLockerDecorator() {
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