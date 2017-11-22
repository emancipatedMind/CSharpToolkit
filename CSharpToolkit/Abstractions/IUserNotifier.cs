namespace CSharpToolkit.Abstractions {
    using System;
    using EventArgs;
    public interface IUserNotifier {
        event EventHandler<GenericEventArgs<string, Urgency>> Notify;
    }
}