namespace CSharpToolkit.Abstractions.DataAccess {
    using System.Data;
    public interface ICachedTableProvider {
        DataTable Table { get; }
    }
}
