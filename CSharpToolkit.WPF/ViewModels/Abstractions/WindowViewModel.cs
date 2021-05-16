namespace CSharpToolkit.ViewModels.Abstractions {
    using System;
    using System.Threading.Tasks;
    using CSharpToolkit.Extensions;
    using CSharpToolkit.Utilities;
    using CSharpToolkit.Utilities.Abstractions;
    using CSharpToolkit.Utilities.EventArgs;
    using CSharpToolkit.XAML;
    public abstract class WindowViewModel : ValidationBase, IViewModelStateProvider, IParentViewModel, IDisposable  {

        bool _disposedValue = false;
        SafeReferenceLockerDecorator _safeFamilialLocker = new SafeReferenceLockerDecorator();
        LockerProxy _familialLockerProxy = new LockerProxy();

        SafeReferenceLockerDecorator _safeApplicationLocker = new SafeReferenceLockerDecorator();
        LockerProxy _applicationRestrictorProxy = new LockerProxy();

        ViewModelState _state;

        object _token = new object();

        Locker _criticalLocker = new Locker();
        Locker _noncriticalLocker = new Locker();

        public WindowViewModel() {
            _safeFamilialLocker.LockStatusChanged += FamilialLocker_LockStatusChanged;
            _safeApplicationLocker.LockStatusChanged += ApplicationLocker_LockStatusChanged;

            _familialLockerProxy.CurrentStatusCallback = () => _safeFamilialLocker.CurrentStatus;
            _familialLockerProxy.LockRequested += ProcessProxyRequest(_safeFamilialLocker.RequestLock);
            _familialLockerProxy.UnLockRequested += ProcessProxyRequest(_safeFamilialLocker.RequestUnlock);

            _applicationRestrictorProxy.CurrentStatusCallback = () => _safeApplicationLocker.CurrentStatus;
            _applicationRestrictorProxy.LockRequested += ProcessProxyRequest(_safeApplicationLocker.RequestLock);
            _applicationRestrictorProxy.UnLockRequested += ProcessProxyRequest(_safeApplicationLocker.RequestUnlock);

            _criticalLocker.LockStatusChanged += PersonalLocker_LockStatusChanged;
            _noncriticalLocker.LockStatusChanged += PersonalLocker_LockStatusChanged;
        }

        public ViewModelState State {
            get { return _state; }
            private set {
                ViewModelState oldState = _state;
                if (Perform.ReplaceIfDifferent(ref _state, value).WasSuccessful) {
                    OnPropertyChanged();
                    OnStateChanged(oldState);
                }
            }
        }

        public ILocker FamilialLocker {
            protected get { return _familialLockerProxy; }
            set {
                var oldValue = _safeFamilialLocker.Component;
                if (Perform.ReplaceIfDifferent(_safeFamilialLocker, nameof(SafeReferenceLockerDecorator.Component), value).WasSuccessful) {
                    OnPropertyChanged();
                    OnFamilialLockerChanged(_safeFamilialLocker.Component, oldValue);
                    OnLockStatusChanged(_familialLockerProxy.CurrentStatus, ApplicationRestrictor.CurrentStatus);
                }
            }
        }

        public ILocker ApplicationRestrictor {
            protected get { return _applicationRestrictorProxy; }
            set {
                var oldValue = _safeApplicationLocker.Component;
                if (Perform.ReplaceIfDifferent(_safeApplicationLocker, nameof(SafeReferenceLockerDecorator.Component), value).WasSuccessful) {
                    OnPropertyChanged();
                    OnFormRestrictorChanged(_safeApplicationLocker.Component, oldValue);
                    OnLockStatusChanged(FamilialLocker.CurrentStatus, _applicationRestrictorProxy.CurrentStatus);
                }
            }
        }

        public ILocker GetFamilialLocker() => _safeFamilialLocker;
        public ILocker GetFormRestrictor() => _safeApplicationLocker;

        public Task RunOperationThroughCriticalState(Func<Task> method) =>
            _criticalLocker.LockForDurationAsync(method);
        public Task<T> RunOperationThroughCriticalState<T>(Func<Task<T>> method) =>
            _criticalLocker.LockForDurationAsync(method);
        public Task RunOperationThroughNonCriticalState(Func<Task> method) =>
            _noncriticalLocker.LockForDurationAsync(method);
        public Task<T> RunOperationThroughNonCriticalState<T>(Func<Task<T>> method) =>
            _noncriticalLocker.LockForDurationAsync(method);

        public void RequestCriticalLock(object token) =>
            _criticalLocker.RequestLock(token);
        public void RequestNonCriticalLock(object token) =>
            _noncriticalLocker.RequestLock(token);
        public void RequestCriticalUnlock(object token) =>
            _criticalLocker.RequestUnlock(token);
        public void RequestNonCriticalUnlock(object token) =>
            _noncriticalLocker.RequestUnlock(token);

        protected virtual void OnStateChanged(ViewModelState oldState) {
            switch(State) {
                case ViewModelState.Idle:
                case ViewModelState.Restricted:
                    FamilialLocker.RequestUnlock(_token);
                    ApplicationRestrictor.RequestUnlock(_token);
                    OnLockStatusChanged(FamilialLocker.CurrentStatus, ApplicationRestrictor.CurrentStatus);
                break;
                case ViewModelState.NonCriticalOperation:
                    FamilialLocker.RequestLock(_token);
                break;
                case ViewModelState.CriticalOperation:
                    FamilialLocker.RequestLock(_token);
                    ApplicationRestrictor.RequestLock(_token);
                break;
            }
        }

        protected virtual void OnFormRestrictorChanged(ILocker formRestrictor, ILocker oldValue) {
            if (State == ViewModelState.CriticalOperation) {
                oldValue?.RequestUnlock(_token);
                formRestrictor?.RequestLock(_token);
            }
        }

        protected virtual void OnFamilialLockerChanged(ILocker familialLocker, ILocker oldValue) {
            if (State == ViewModelState.CriticalOperation || State == ViewModelState.NonCriticalOperation) {
                oldValue?.RequestUnlock(_token);
                familialLocker?.RequestLock(_token);
            }
        }

        protected virtual void OnLockStatusChanged(LockStatus familialStatus, LockStatus formRestrictorStatus) {
            if (State == ViewModelState.CriticalOperation || State == ViewModelState.NonCriticalOperation)
                return;
            if (familialStatus == LockStatus.Locked)
                State = ViewModelState.FamilialOperation;
            else if (formRestrictorStatus == LockStatus.Locked)
                State = ViewModelState.Restricted;
            else
                State = ViewModelState.Idle;
        }

        private void FamilialLocker_LockStatusChanged(object sender, GenericEventArgs<LockStatus> e) {
            OnLockStatusChanged(FamilialLocker.CurrentStatus, ApplicationRestrictor.CurrentStatus);
            _familialLockerProxy.FireLockStatusChanged(e.Data);
        }
        private void ApplicationLocker_LockStatusChanged(object sender, GenericEventArgs<LockStatus> e) {
            OnLockStatusChanged(FamilialLocker.CurrentStatus, ApplicationRestrictor.CurrentStatus);
            _applicationRestrictorProxy.FireLockStatusChanged(e.Data);
        }

        private EventHandler<GenericEventArgs<object>> ProcessProxyRequest(Action<object> request) =>
            (s, e) => {
                _safeFamilialLocker.LockStatusChanged -= FamilialLocker_LockStatusChanged;
                _safeApplicationLocker.LockStatusChanged -= ApplicationLocker_LockStatusChanged;

                request(e.Data);

                _safeFamilialLocker.LockStatusChanged += FamilialLocker_LockStatusChanged;
                _safeApplicationLocker.LockStatusChanged += ApplicationLocker_LockStatusChanged;
            };

        private void PersonalLocker_LockStatusChanged(object sender, GenericEventArgs<LockStatus> e) {
            if (_criticalLocker.IsLocked())
                State = ViewModelState.CriticalOperation;
            else if (_noncriticalLocker.IsLocked())
                State = ViewModelState.NonCriticalOperation;
            else
                State = ViewModelState.Idle;
        }

        protected override void Dispose(bool disposing) {
            if (!_disposedValue) {
                if (disposing) {
                    State = ViewModelState.Idle;
                    ApplicationRestrictor.RequestUnlock(_token);
                    FamilialLocker.RequestUnlock(_token);

                    _safeFamilialLocker.Dispose();
                    _familialLockerProxy.Dispose();
                    _safeApplicationLocker.Dispose();
                    _applicationRestrictorProxy.Dispose();
                }
                _disposedValue = true;
            }
            base.Dispose(disposing);
        }

    }
}
