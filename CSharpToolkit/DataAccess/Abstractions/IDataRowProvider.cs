namespace CSharpToolkit.DataAccess.Abstractions {
    using System.Collections.Generic;
    using System.Data;
    using Utilities;
    /// <summary>
    /// Implemented by class who may fulfill a query.
    /// </summary>
    public interface IDataRowProvider {
        /// <summary>
        /// Performs submission of query.
        /// </summary>
        /// <param name="sql">Query text.</param>
        /// <param name="commandType">Type of command.</param>
        /// <param name="parameters">Parameter for query.</param>
        /// <returns>Operation result with a list of DataRows.</returns>
        OperationResult<List<DataRow>> SubmitQuery(string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters);
    }
}