namespace CSharpToolkit.Abstractions.DataAccess {
    using System.Collections.Generic;
    using Utilities;
    public interface IDataOperator : IDataRowProvider {
        OperationResult PerformDataOperation(string command);
        OperationResult PerformDataOperation(string command, IEnumerable<KeyValuePair<string, object>> parameters);
    }
}