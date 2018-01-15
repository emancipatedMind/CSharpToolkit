namespace CSharpToolkit.Validation.Abstractions {
    using Utilities;
    /// <summary>
    /// Denotes class who can participate in Validation.
    /// </summary>
    /// <typeparam name="T">Data type to be validated.</typeparam>
    public interface IValidate<T> {
        /// <summary>
        /// Perform validation.
        /// </summary>
        /// <param name="order">Object to be validated.</param>
        /// <returns>Operation result.</returns>
        OperationResult Validate(T order); 
    }
}