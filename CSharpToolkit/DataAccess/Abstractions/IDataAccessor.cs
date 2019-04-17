namespace CSharpToolkit.DataAccess.Abstractions {
    using System.Collections.Generic;
    using System.Data;
    using Utilities;
    /// <summary>
    /// Implemented by class who may fulfill a query.
    /// </summary>
    public interface IDataAccessor : IDataRowProvider {
        /// <summary>
        /// Performs submission of query to return a single value.
        /// </summary>
        /// <param name="sql">Query text.</param>
        /// <param name="commandType">Type of command.</param>
        /// <param name="parameters">Parameters for query.</param>
        /// <returns>An <see cref="OperationResult"/> containing the value of the first field found by query.</returns>
        OperationResult<object> SubmitScalarQuery(string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters);

        /// <summary>
        /// Performs a data operation that returns number of rows affected.
        /// </summary>
        /// <param name="sql">Query text.</param>
        /// <param name="commandType">Type of command.</param>
        /// <param name="parameters">Parameters for query.</param>
        /// <returns>An <see cref="OperationResult"/> containing number of rows affected.</returns>
        OperationResult<int> PerformDataOperation(string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters);
    }
}