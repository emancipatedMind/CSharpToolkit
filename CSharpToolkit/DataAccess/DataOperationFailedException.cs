namespace CSharpToolkit.DataAccess {
    using System;
    /// <summary>
    /// An exception that represents that a data operation has failed.
    /// </summary>
    [Serializable]
    public class DataOperationFailedException : Exception {
        /// <summary>
        /// Instantiates the DataOperationFailedException class.
        /// </summary>
        public DataOperationFailedException() { }
        /// <summary>
        /// Instantiates the DataOperationFailedException class.
        /// </summary>
        /// <param name="message">The error message which explains the reason for the exception.</param>
        public DataOperationFailedException(string message) : base(message) { }
        /// <summary>
        /// Instantiates the DataOperationFailedException class.
        /// </summary>
        /// <param name="message">The error message which explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of this exception.</param>
        public DataOperationFailedException(string message, Exception inner) : base(message, inner) { }
        /// <summary>
        /// Instantiates the DataOperationFailedException class.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data for the exception being thrown.</param>
        /// <param name="context">The object that contains contextual information about the source or destination.</param>
        protected DataOperationFailedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}