namespace CSharpToolkit.EventArgs {
    public class GenericEventArgs<T> : System.EventArgs {
        public T Data { get; private set; }
        public GenericEventArgs(T data) {
            Data = data;
        }
    }
}