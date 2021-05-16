namespace CSharpToolkit.Utilities.Abstractions {
    /// <summary>
    /// Adorned by a class that can import a type.
    /// </summary>
    /// <typeparam name="TImport">The type to import.</typeparam>
    public interface IImporterWithResult<TImport> {

        /// <summary>
        /// Perform import of type.
        /// </summary>
        /// <param name="obj">Member to import.</param>
        /// <returns><see cref="OperationResult"/> denoting whether import was successful.</returns>
        OperationResult Import(TImport obj);
    }
}
