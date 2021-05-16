namespace CSharpToolkit.Utilities.Abstractions {
    using System.Collections.Generic;
    public interface IVMLocator<TViewModel> : ILocator {
        IList<TViewModel> ModifyableItems { get; }
    }
}