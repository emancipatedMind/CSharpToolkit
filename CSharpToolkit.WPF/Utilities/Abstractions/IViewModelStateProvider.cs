namespace CSharpToolkit.Utilities.Abstractions {
    using CSharpToolkit.Utilities.EventArgs;
    using System;
    public interface IViewModelStateProvider {
        ViewModelState State { get; }
    }
}
