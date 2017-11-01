namespace CSharpToolkit.EventArgs {
    public class GenericEventArgs<T> : System.EventArgs {
        public T Data { get; private set; }
        public GenericEventArgs(T data) {
            Data = data;
        }
    }
    public class GenericEventArgs<T, T2> : System.EventArgs {
        public T Data { get; private set; }
        public T2 SecondaryData { get; private set; }
        public GenericEventArgs(T data, T2 secondaryData) {
            Data = data;
            SecondaryData = secondaryData;
        }
    }
}
