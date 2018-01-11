namespace CSharpToolkit.DataAccess.SQL {
    using System.Linq;
    using System.Collections.Generic;
    using System.Data;
    using Abstractions.DataAccess;
    using Utilities;
    using System.Data.SqlClient;
    /// <summary>
    /// Implementation of IDataRowProvider using SQL.
    /// </summary>
    public class SQLDataRowProvider : IDataRowProvider {

        string _connectionString;

        /// <summary>
        /// Instantiates class. Used for submission of queries with SQL.
        /// </summary>
        /// <param name="connectionString">Connection string for use by connection.</param>
        public SQLDataRowProvider(string connectionString) {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Instantiates SQLDataRowProvider.
        /// </summary>
        /// <param name="builder">Connection string builder which produces connection string for use by connection.</param>
        public SQLDataRowProvider(SqlConnectionStringBuilder builder) {
            _connectionString = builder.ToString();
        }

        /// <summary>
        /// Connection timeout.
        /// </summary>
        public int Timeout { get; set; } = 30;

        /// <summary>
        /// Performs submission of query.
        /// </summary>
        /// <param name="sql">Query text.</param>
        /// <param name="commandType">Type of command.</param>
        /// <param name="parameters">Parameter for query.</param>
        /// <returns>Operation result with a list of DataRows.</returns>
        public OperationResult<List<DataRow>> SubmitQuery(string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters) =>
            Get.OperationResult(() => {
                var command = new SqlCommand {
                    CommandTimeout = Timeout,
                    CommandText = sql,
                    CommandType = commandType
                };
                command.Parameters.AddRange(parameters.Select(p => new SqlParameter(p.Key, p.Value)).ToArray());

                DataTable result = new DataTable();
                using (var connection = new SqlConnection(_connectionString)) {
                    command.Connection = connection;
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                        result.Load(reader);
                }
                return result.AsEnumerable().ToList();
            });
    }
}