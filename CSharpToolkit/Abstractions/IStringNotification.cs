namespace CSharpToolkit.Abstractions {
    using System;
    using EventArgs;
    public interface IStringNotification {
        event EventHandler<GenericEventArgs<string>> Notify;
    }
}