namespace CSharpToolkit.Utilities.Abstractions {
    using System.Collections.Generic;

    public interface IContainsParts<TPart> {
        IList<TPart> Parts { get; }
    }

}
