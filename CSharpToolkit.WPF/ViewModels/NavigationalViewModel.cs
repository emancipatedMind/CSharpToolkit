namespace CSharpToolkit.ViewModels {
    using System.Threading.Tasks;
    using CSharpToolkit.Utilities;
    using CSharpToolkit.XAML;
    using Utilities;
    public class NavigationalViewModel : EntityBase {

        static AwaitableDelegateCommand DefaultCommand = new AwaitableDelegateCommand(() => Task.CompletedTask, () => false);

        ViewModelState _state = ViewModelState.Idle;
        CriticalOperationType _criticalOperationType = CriticalOperationType.None;
        bool _findAvailable = false;
        bool _duplicateAvailable = false;
        bool _newAvailable = true;
        bool _reloadCurrentAvailable = false;

        AwaitableDelegateCommand _back;
        AwaitableDelegateCommand _forward;
        AwaitableDelegateCommand _cancel;
        AwaitableDelegateCommand _confirm;
        AwaitableDelegateCommand _new;
        AwaitableDelegateCommand _modify;
        AwaitableDelegateCommand _find;
        AwaitableDelegateCommand _duplicate;
        AwaitableDelegateCommand _reloadCurrent;
        private bool _disposed;

        public ViewModelState State {
            get { return _state; }
            set { FirePropertyChangedIfDifferent(ref _state, value); }
        }

        public bool ReloadCurrentAvailable {
            get { return _reloadCurrentAvailable; }
            set { FirePropertyChangedIfDifferent(ref _reloadCurrentAvailable, value); }
        }
        public bool DuplicateAvailable {
            get { return _duplicateAvailable; }
            set { FirePropertyChangedIfDifferent(ref _duplicateAvailable, value); }
        }
        public bool FindAvailable {
            get { return _findAvailable; }
            set { FirePropertyChangedIfDifferent(ref _findAvailable, value); }
        }
        public bool NewAvailable {
            get { return _newAvailable; }
            set { FirePropertyChangedIfDifferent(ref _newAvailable, value); }
        }

        public AwaitableDelegateCommand ReloadCurrent {
            get { return _reloadCurrent ?? DefaultCommand; }
            set { FirePropertyChangedIfDifferent(ref _reloadCurrent, value); }
        }
        public AwaitableDelegateCommand Duplicate {
            get { return _duplicate ?? DefaultCommand; }
            set { FirePropertyChangedIfDifferent(ref _duplicate, value); }
        }
        public AwaitableDelegateCommand Back {
            get { return _back ?? DefaultCommand; }
            set { FirePropertyChangedIfDifferent(ref _back, value); }
        }
        public AwaitableDelegateCommand Cancel {
            get { return _cancel ?? DefaultCommand; }
            set { FirePropertyChangedIfDifferent(ref _cancel, value); }
        }
        public AwaitableDelegateCommand Confirm {
            get { return _confirm ?? DefaultCommand; }
            set { FirePropertyChangedIfDifferent(ref _confirm, value); }
        }
        public AwaitableDelegateCommand Find {
            get { return _find ?? DefaultCommand; }
            set { FirePropertyChangedIfDifferent(ref _find, value); }
        }
        public AwaitableDelegateCommand Forward {
            get { return _forward ?? DefaultCommand; }
            set { FirePropertyChangedIfDifferent(ref _forward, value); }
        }
        public AwaitableDelegateCommand Modify {
            get { return _modify ?? DefaultCommand; }
            set { FirePropertyChangedIfDifferent(ref _modify, value); }
        }
        public AwaitableDelegateCommand New {
            get { return _new ?? DefaultCommand; }
            set { FirePropertyChangedIfDifferent(ref _new, value); }
        }
        public CriticalOperationType CriticalOperationType {
            get { return _criticalOperationType; }
            set { FirePropertyChangedIfDifferent(ref _criticalOperationType, value); }
        }

        protected override void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    _back = null;
                    _forward = null;
                    _cancel = null;
                    _confirm = null;
                    _new = null;
                    _modify = null;
                    _find = null;
                    _duplicate = null;
                    _reloadCurrent = null;

                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }

    }
}
