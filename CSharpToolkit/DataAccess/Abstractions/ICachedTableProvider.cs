namespace CSharpToolkit.DataAccess.Abstractions {
    using System.Data;
    /// <summary>
    /// Implemented by class who contains a cached table.
    /// </summary>
    public interface ICachedTableProvider {
        /// <summary>
        /// Cached table.
        /// </summary>
        DataTable Table { get; }
    }
}
