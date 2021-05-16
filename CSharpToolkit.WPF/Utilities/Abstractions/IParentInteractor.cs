namespace CSharpToolkit.Utilities.Abstractions {
    public interface IParentInteractor {
        ModificationMode ModificationMode { get; }
        event System.EventHandler ModelRefreshRequested;
    }
}
