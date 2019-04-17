namespace CSharpToolkit.Utilities {
    using System;
    [Serializable]
    public class NullSynchronizationException : Exception {
        public NullSynchronizationException() { }
        public NullSynchronizationException(string message) : base(message) { }
        public NullSynchronizationException(string message, Exception inner) : base(message, inner) { }
        protected NullSynchronizationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}