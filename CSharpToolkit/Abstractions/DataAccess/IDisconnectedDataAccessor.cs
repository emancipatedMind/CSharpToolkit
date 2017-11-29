namespace CSharpToolkit.Abstractions.DataAccess {
    using System.Data;
    using Utilities;
    public interface IDisconnectedDataAccessor {
        OperationResult<DataTable> GetDataTable(string sql);
    }
}
