namespace CSharpToolkit.DataAccess.Abstractions {
    using System.Collections.Generic;
    using System.Data;
    using Utilities;
    /// <summary>
    /// Implemented by class who may fulfill a query to receive data.
    /// </summary>
    public interface IDataRowProvider {
        /// <summary>
        /// Performs submission of query to receive data rows;
        /// </summary>
        /// <param name="sql">Query text.</param>
        /// <param name="commandType">Type of command.</param>
        /// <param name="parameters">Parameters for query.</param>
        /// <returns>An <see cref="OperationResult"/> containing a <see cref="List{T}"/> of <see cref="DataRow"/>.</returns>
        OperationResult<List<DataRow>> SubmitQuery(string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters);

        /// <summary>
        /// Performs submission of query to receive datasets.
        /// </summary>
        /// <param name="sql">Query text.</param>
        /// <param name="commandType">Type of command.</param>
        /// <param name="parameters">Parameters for query.</param>
        /// <returns>An <see cref="OperationResult"/> containing a <see cref="DataSet"/>.</returns>
        OperationResult<DataSet> SubmitQueryForDataSet(string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters);
    }
}