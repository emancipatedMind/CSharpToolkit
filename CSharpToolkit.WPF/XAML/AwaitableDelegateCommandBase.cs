namespace CSharpToolkit.XAML {
    using System;
    using Utilities.EventArgs;
    public abstract class AwaitableDelegateCommandBase {
        public static event EventHandler<GenericEventArgs<Exception>> UnHandledException;

        protected static bool OnUnHandledException(object sender, Exception ex) {
            if (UnHandledException == null)
                return false;
            UnHandledException?.Invoke(sender, new GenericEventArgs<Exception>(ex));
            return true;
        }
    }
}