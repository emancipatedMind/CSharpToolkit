namespace CSharpToolkit.Abstractions {
    public interface IReflectionOperation<T> {
        T Perform(object obj);
    }
}