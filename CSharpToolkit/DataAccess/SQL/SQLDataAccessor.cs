namespace CSharpToolkit.DataAccess.SQL {
    using System.Linq;
    using System.Collections.Generic;
    using System.Data;
    using Abstractions;
    using Utilities;
    using System.Data.SqlClient;
    using System;

    /// <summary>
    /// Implementation of <see cref="IDataAccessor"/> using SQL. Used for submission of queries with SQL.
    /// </summary>
    public class SQLDataAccessor : SQLDataRowProvider, IDataAccessor {

        /// <summary>
        /// Instantiates SQLDataAccessor. Used for submission of queries with SQL.
        /// </summary>
        /// <param name="builder">Connection string builder which produces connection string for use by connection.</param>
        public SQLDataAccessor(SqlConnectionStringBuilder builder) : base(builder) { }

        /// <summary>
        /// Instantiates SQLDataAccessor. Used for submission of queries with SQL.
        /// </summary>
        /// <param name="connectionString">Connection string for use by connection.</param>
        public SQLDataAccessor(string connectionString) : base(connectionString) { }

        /// <summary>
        /// Performs a data operation that returns number of rows affected.
        /// </summary>
        /// <param name="sql">Query text.</param>
        /// <param name="commandType">Type of command.</param>
        /// <param name="parameters">Parameters for query.</param>
        /// <returns>Operation result containing number of rows affected.</returns>
        public OperationResult<int> PerformDataOperation(string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters) =>
            PerformDataBaseCall(sql, commandType, parameters, command => command.ExecuteNonQuery());

        /// <summary>
        /// Performs submission of query to return a single value.
        /// </summary>
        /// <param name="sql">Query text.</param>
        /// <param name="commandType">Type of command.</param>
        /// <param name="parameters">Parameters for query.</param>
        /// <returns>Operation result containing the first field of the first row, or a single value.</returns>
        public OperationResult<object> SubmitScalarQuery(string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters) =>
            PerformDataBaseCall(sql, commandType, parameters, command => command.ExecuteScalar());

    }
}