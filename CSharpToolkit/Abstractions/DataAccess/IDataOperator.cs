namespace CSharpToolkit.Abstractions.DataAccess {
    using System.Collections.Generic;
    using Utilities;
    /// <summary>
    /// Implemented by a class who performs operations on data.
    /// </summary>
    public interface IDataOperator : IDataRowProvider {
        /// <summary>
        /// Perform data operation.
        /// </summary>
        /// <param name="command">Command to be submitted.</param>
        /// <param name="parameters">Parameters needed by command.</param>
        /// <returns>Operation result.</returns>
        OperationResult PerformDataOperation(string command, IEnumerable<KeyValuePair<string, object>> parameters);
    }
}