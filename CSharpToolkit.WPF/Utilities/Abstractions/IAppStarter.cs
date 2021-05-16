namespace CSharpToolkit.Utilities.Abstractions {
    using System;
    using CSharpToolkit.Utilities.EventArgs;
    public interface IAppStarter<TStartUpInfo> {
        event EventHandler<GenericEventArgs<TStartUpInfo>> StartUpSuccessful;
        event EventHandler StartUpFailed;
        void Start();
    }
}
