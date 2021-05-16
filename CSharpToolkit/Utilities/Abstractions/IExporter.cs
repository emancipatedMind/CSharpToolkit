namespace CSharpToolkit.Utilities.Abstractions {
    /// <summary>
    /// Adorned by a class that can extract a class.
    /// </summary>
    /// <typeparam name="TExport">The type to extract.</typeparam>
    public interface IExporter<TExport> {
        /// <summary>
        /// Perform the extraction.
        /// </summary>
        /// <returns>The extracted object.</returns>
        TExport Export();
    }
}