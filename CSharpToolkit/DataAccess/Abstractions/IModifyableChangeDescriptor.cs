namespace CSharpToolkit.DataAccess.Abstractions {
    /// <summary>
    /// Implemented by a class who is modifyable allowing data to be saved, reset, and keeps a running list of properties that have been changed.
    /// </summary>
    public interface IModifyableChangeDescriptor : IChangeDescriptor, IModifyable {
    }
}