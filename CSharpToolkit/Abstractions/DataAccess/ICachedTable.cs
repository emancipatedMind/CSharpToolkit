namespace CSharpToolkit.Abstractions.DataAccess {
    using Utilities;
    using System.Data;
    public interface ICachedTable {
        OperationResult<DataTable> Table { get; }
    }
}
