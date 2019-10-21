namespace CSharpToolkit.XAML {
    using System;
    using System.Threading.Tasks;
    using System.Linq;
    using Utilities;
    using System.Collections;
    using Utilities.Abstractions;
    using CSharpToolkit.Extensions;
    using System.Windows.Threading;
    using Utilities.EventArgs;

    public class TokenTimer : IDisposable {

        DispatcherTimer _dispatcherTimer;
        Locker _locker = new Locker();
        bool _initialized;
        private bool _disposed;
        private Status _status;
        TimeSpan _minimumInterval = new TimeSpan(0, 0, 1);
        TimeSpan _maximumInterval = new TimeSpan(0, 0, 1800);

        public TokenTimer() : this(new TimeSpan(0, 0, 5)) { }
        public TokenTimer(TimeSpan interval) {
            _dispatcherTimer = new DispatcherTimer { Interval = interval };
            _dispatcherTimer.Tick += DispatcherTimer_Tick;

            _locker.LockStatusChanged += (s, e) => {
                _dispatcherTimer.Stop();
                var status = Status.Stopped;
                if (_locker.IsFree()) {
                    _dispatcherTimer.Start();
                    status = Status.Running;
                }

                CurrentStatus = status;
            };
        }

        private async void DispatcherTimer_Tick(object sender, EventArgs e) {
            if (_locker.IsFree()) {
                var function = Tick_Callback;
                if (function != null)
                    await function();
            }
        }

        public Func<Task> Tick_Callback { get; set; }

        public void Initialize() {
            if (_initialized)
                return;
            _initialized = true;

            _dispatcherTimer.Start();
        }

        public Status CurrentStatus {
            get { return _status; }
            protected set {
                if (Perform.ReplaceIfDifferent(ref _status, value).WasSuccessful) {
                    TimerStatusChanged?.Invoke(this, new GenericEventArgs<Status>(value));
                } 
            }
        }

        public void RequestStop(object token) =>
            _locker.RequestLock(token);

        public void RequestStart(object token) =>
            _locker.RequestUnlock(token);

        public TimeSpan MinimumInterval {
            get { return _minimumInterval; }
            set {
                if (Perform.ReplaceIfDifferent(ref _minimumInterval, value).WasSuccessful) {
                    Interval = Interval;
                }
            }
        }

        public TimeSpan MaximumInterval {
            get { return _maximumInterval; }
            set {
                if (Perform.ReplaceIfDifferent(ref _maximumInterval, value).WasSuccessful) {
                    Interval = Interval;
                }
            }
        }

        public TimeSpan Interval {
            get { return _dispatcherTimer.Interval; }
            set {
                var token = new object();
                _locker.RequestLock(token);
                if (value < MinimumInterval)
                    value = MinimumInterval;
                else if (value > MaximumInterval)
                    value = MaximumInterval;
                _dispatcherTimer.Interval = value;
                _locker.RequestUnlock(token);
            }
        }

        public event EventHandler<GenericEventArgs<Status>> TimerStatusChanged;
        public event EventHandler IdleIntervalExceeded;

        public enum Status {
            Stopped,
            Running,
        }

        public void Dispose() {
            if (!_disposed) {
                _disposed = true;
                _dispatcherTimer.Stop();
                _dispatcherTimer.Tick -= DispatcherTimer_Tick;
                TimerStatusChanged = null;
                IdleIntervalExceeded = null;
                Tick_Callback = null;
                _locker.Dispose();
            }
        }

        public Task StopForDurationAsync(Func<Task> method) =>
            StopForDurationAsync(new object(), method);

        public Task StopForDurationAsync(object token, Func<Task> method) =>
            _locker.LockForDurationAsync(token, method);

    }
}
