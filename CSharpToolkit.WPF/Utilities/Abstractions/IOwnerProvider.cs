namespace CSharpToolkit.Utilities.Abstractions {
    using System.Windows;
    public interface IOwnerProvider {
        System.Windows.Window Owner { get; set; }
    }
}
