namespace CSharpToolkit.Validation {
    /// <summary>
    /// An exception used to represent a failed validation.
    /// </summary>
    [System.Serializable]
    public class ValidationFailedException : System.Exception {
        /// <summary>
        /// Denotes that validation has failed.
        /// </summary>
        public ValidationFailedException() { }
        /// <summary>
        /// Denotes that validation has failed.
        /// </summary>
        /// <param name="message">The message on why the validation failed.</param>
        public ValidationFailedException(string message) : base(message) { }
        /// <summary>
        /// Denotes that validation has failed.
        /// </summary>
        /// <param name="message">The message on why the validation failed.</param>
        /// <param name="inner">The exception that is the cause of this exception.</param>
        public ValidationFailedException(string message, System.Exception inner) : base(message, inner) { }
        /// <summary>
        /// Constructor used to serialize exception.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data for the exception being thrown.</param>
        /// <param name="context">The object that contains contextual information about the source or destination.</param>
        protected ValidationFailedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}