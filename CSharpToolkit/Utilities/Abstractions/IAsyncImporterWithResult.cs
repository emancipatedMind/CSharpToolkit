namespace CSharpToolkit.Utilities.Abstractions {
    using System.Threading.Tasks;
    /// <summary>
    /// Adorned by a class that can import a type.
    /// </summary>
    /// <typeparam name="TImport">The type to import.</typeparam>
    public interface IAsyncImporterWithResult<TImport> {

        /// <summary>
        /// Perform import of type.
        /// </summary>
        /// <param name="obj">Member to import.</param>
        /// <returns><see cref="OperationResult"/> wrapped in <see cref="Task"/> denoting whether import was successful.</returns>
        Task<OperationResult> ImportAsync(TImport obj);
    }
}