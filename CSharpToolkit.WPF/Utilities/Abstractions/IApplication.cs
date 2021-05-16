namespace CSharpToolkit.Utilities.Abstractions {
    using System.Windows;
    using CSharpToolkit.Logging.Abstractions;
    public interface IApplication<TDataStore> : IApplication {
        TDataStore DataStore { get; }
    }
    public interface IApplication {
        void SetUpAdminInteractor(IAdminInteractorConfigurer interactor);
        void SetUpWindow(Window window);
        DAIDB.Validation.StandardValidator StandardValidator { get; }
        CSharpToolkit.Validation.Abstractions.IValidate<string> LocatorValidator { get; }
        CSharpToolkit.Utilities.Abstractions.ILocker ApplicationLocker { get; }
        IEmailLoggerAsync Emailer { get; }
        IGeoServices GeoServiceProvider { get; }
        IExceptionLogger ErrorLogger { get; }
        IVINServices VINServicesProvider { get; }
    }
}
