namespace CSharpToolkit.DataAccess {
    using System;
    /// <summary>
    /// An exception that represents an empty result set.
    /// </summary>
    [Serializable]
    public class EmptyResultSetException : Exception {
        /// <summary>
        /// Denotes that result set returned was empty.
        /// </summary>
        public EmptyResultSetException() { }
        /// <summary>
        /// Denotes that result set returned was empty.
        /// </summary>
        /// <param name="message">The error message which explains the reason for the exception.</param>
        public EmptyResultSetException(string message) : base(message) { }
        /// <summary>
        /// Denotes that result set returned was empty.
        /// </summary>
        /// <param name="message">The error message which explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of this exception.</param>
        public EmptyResultSetException(string message, Exception inner) : base(message, inner) { }
        /// <summary>
        /// Constructor used to serialize exception.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data for the exception being thrown.</param>
        /// <param name="context">The object that contains contextual information about the source or destination.</param>
        protected EmptyResultSetException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}