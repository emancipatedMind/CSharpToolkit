namespace CSharpToolkit.Utilities.Exceptions {
    public class EmptyFileException : System.Exception {
        public EmptyFileException() { }
        public EmptyFileException(string message) : base(message) { }
        public EmptyFileException(string message, System.Exception inner) : base(message, inner) { }
        protected EmptyFileException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}