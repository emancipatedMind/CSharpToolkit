namespace CSharpToolkit.Validation {
    using Utilities;
    /// <summary>
    /// A null implementation of the IValidate{T} interface. This class always returns that the validation has passed.
    /// </summary>
    /// <typeparam name="T">The type of item to validate.</typeparam>
    public class NullValidator<T> : Abstractions.IValidate<T> {
        static NullValidator<T> _instance;
        /// <summary>
        /// Implementation of the Singleton patterns. Returns an instance of the NullValidator{T} class. Implemented lazily.
        /// </summary>
        public static NullValidator<T> Instance => _instance ?? (_instance = new NullValidator<T>());

        private NullValidator() { }

        /// <summary>
        /// Returns that validation has passed.
        /// </summary>
        /// <param name="order">The item to validate.</param>
        /// <returns>A successful validation.</returns>
        public OperationResult Validate(T order) => new OperationResult();
    } 
}