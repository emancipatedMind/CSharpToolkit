namespace CSharpToolkit.Utilities.Abstractions {
    public interface IReflectionOperation<T> {
        T Perform(object obj);
    }
}