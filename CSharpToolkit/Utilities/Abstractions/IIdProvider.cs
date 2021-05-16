namespace CSharpToolkit.Utilities.Abstractions {
    /// <summary>
    /// Adorned by a class who has specific int property which represents an Id.
    /// </summary>
    public interface IIdProvider {
        /// <summary>
        /// The Id of the instance.
        /// </summary>
        int Id { get; }
    }
}