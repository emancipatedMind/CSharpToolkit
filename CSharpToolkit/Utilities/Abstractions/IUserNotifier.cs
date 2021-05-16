namespace CSharpToolkit.Utilities.Abstractions {
    using System;
    using EventArgs;
    /// <summary>
    /// Adorned by a class who may send string notifications.
    /// </summary>
    public interface IUserNotifier {
        /// <summary>
        /// Raised whenever a notification is made by the adorning class.
        /// </summary>
        event EventHandler<GenericEventArgs<string, Urgency>> Notify;
    }
}