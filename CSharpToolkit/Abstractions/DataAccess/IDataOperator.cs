namespace CSharpToolkit.Abstractions.DataAccess {
    using Utilities;
    public interface IDataOperator : IDataRowProvider {
        OperationResult PerformDataOperation(string command);
    }
}