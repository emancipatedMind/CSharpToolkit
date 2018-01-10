namespace CSharpToolkit.Abstractions {
    using Utilities;
    public interface IValidate<T> {
        OperationResult Validate(T order); 
    }
}