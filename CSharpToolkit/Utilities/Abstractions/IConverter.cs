namespace CSharpToolkit.Utilities.Abstractions {
    /// <summary>
    /// Adorned by a class who can convert one type into another.
    /// </summary>
    /// <typeparam name="TInput">The input type.</typeparam>
    /// <typeparam name="TResult">The output type.</typeparam>
    public interface IConverter <TInput, TResult> {
        /// <summary>
        /// Converts one type into another.
        /// </summary>
        /// <param name="input">The type to convert.</param>
        /// <returns>The converted type.</returns>
        TResult Convert(TInput input);
    }
}
