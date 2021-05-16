namespace CSharpToolkit.Utilities.Abstractions {
    /// <summary>
    /// Adorned by a class that can import a type.
    /// </summary>
    /// <typeparam name="TImport">The type to import.</typeparam>
    public interface IImporter<TImport> {
        /// <summary>
        /// Perform import of type.
        /// </summary>
        /// <param name="obj">Member to import.</param>
        void Import(TImport obj);
    }
}
