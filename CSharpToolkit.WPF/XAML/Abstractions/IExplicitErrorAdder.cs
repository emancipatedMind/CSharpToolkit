namespace CSharpToolkit.XAML.Abstractions {
    using System;
    public interface IExplicitErrorAdder {
        void AddError(string propertyName, Exception ex);
    }
}