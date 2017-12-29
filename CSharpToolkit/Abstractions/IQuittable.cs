namespace CSharpToolkit.Abstractions {
    using System.Windows.Input;
    public interface IQuittable {
        ICommand Quit { get; }
    }
}