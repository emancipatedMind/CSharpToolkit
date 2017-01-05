namespace CSharpToolkit.EventArgs {
    using System;
    public class NotificationEventArgs : EventArgs {
        public string Message { get; private set; }

        public NotificationEventArgs(string message) {
            Message = message;
        }
    }
    public class NotificationEventArgs<T> : NotificationEventArgs {
        public T Data { get; private set; }
        public NotificationEventArgs(string message, T data) : base(message) {
            Data = data;
        }
    }
    public class NotificationEventArgs<T, T2> : NotificationEventArgs<T> {
        public Action<T2> Action { get; private set; }
        public NotificationEventArgs(string message, T data, Action<T2> action) : base(message, data) {
            Action = action;
        }
    }
}