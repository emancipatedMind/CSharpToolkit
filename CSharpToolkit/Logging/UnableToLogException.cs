namespace CSharpToolkit.Logging {

    [System.Serializable]
    public class UnableToLogException : System.Exception {
        public UnableToLogException() { }
        public UnableToLogException(string message) : base(message) { }
        public UnableToLogException(string message, System.Exception inner) : base(message, inner) { }
        protected UnableToLogException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

}