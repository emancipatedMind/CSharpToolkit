namespace CSharpToolkit.Utilities {
    using System.Collections.Generic;
    using System;
    using EventArgs;
    using Abstractions;
    /// <summary>
    /// This decorator marshals all requests to <see cref="Component"/>. The functionality given is all calls to <see cref="RequestUnlock(object)"/>, and <see cref="RequestLock(object)"/> will have listeners removed for duration.
    /// </summary>
    public class NonRecursiveLockerDecorator : ILocker {

        ILocker _component;

        /// <summary>
        /// Instantiates the <see cref="NonRecursiveLockerDecorator"/> class.
        /// </summary>
        public NonRecursiveLockerDecorator() { }

        /// <summary>
        /// Instantiates the <see cref="NonRecursiveLockerDecorator"/> class.
        /// </summary>
        /// <param name="component">The inner component through which requests are marshaled.</param>
        public NonRecursiveLockerDecorator(ILocker component) {
            component.LockStatusChanged += Component_LockStatusChanged;
        }

        private void Component_LockStatusChanged(object sender, GenericEventArgs<LockStatus> e) =>
            LockStatusChanged?.Invoke(sender, e);

        /// <summary>
        /// The object through which all requests are marshaled.
        /// </summary>
        public ILocker Component {
            get { return _component; }
            set {
                var oldComponent = _component;
                if (Perform.ReplaceIfDifferent(ref _component, value).WasSuccessful) {
                    SwapComponent(_component, oldComponent);
                }
            }
        }

        private void SwapComponent(ILocker newComponent, ILocker oldComponent) {
            if (newComponent != null) {
                newComponent.LockStatusChanged += Component_LockStatusChanged;
            } 
            if (oldComponent != null) {
                oldComponent.LockStatusChanged -= Component_LockStatusChanged;
            } 
        }

        /// <summary>
        /// The <see cref="CurrentStatus"/> of the marshaled <see cref="Component"/>.
        /// </summary>
        public LockStatus CurrentStatus => _component?.CurrentStatus ?? LockStatus.Free;

        /// <summary>
        /// Linked to the <see cref="LockStatusChanged"/> of the <see cref="Component"/>. This event will be disabled when calling either <see cref="RequestLock(object)"/>, or <see cref="RequestUnlock(object)"/>. 
        /// </summary>
        public event EventHandler<GenericEventArgs<LockStatus>> LockStatusChanged;

        /// <summary>
        /// Passes request to inner <see cref="Component"/>. The <see cref="LockStatusChanged"/> event is inactive during request.
        /// </summary>
        /// <param name="token">The lock token.</param>
        public void RequestLock(object token) {
            if (_component == null)
                return;

            _component.LockStatusChanged -= Component_LockStatusChanged;
            _component.RequestLock(token);
            _component.LockStatusChanged += Component_LockStatusChanged;
        }

        /// <summary>
        /// Passes request to inner <see cref="Component"/>. The <see cref="LockStatusChanged"/> event is inactive during request.
        /// </summary>
        /// <param name="token">The unlock token.</param>
        public void RequestUnlock(object token) {
            if (_component == null)
                return;

            _component.LockStatusChanged -= Component_LockStatusChanged;
            _component.RequestUnlock(token);
            _component.LockStatusChanged += Component_LockStatusChanged;
        }

    }
}