namespace CSharpToolkit.DataAccess.SQL {
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Data.SqlTypes;
    using System.Linq;
    using Abstractions;
    using Utilities;

    /// <summary>
    /// Implementation of <see cref="IDataRowProvider"/> using SQL. Used for retrieval of data with SQL.
    /// </summary>
    public class SQLDataRowProvider : IDataRowProvider {

        string _connectionString;

        /// <summary>
        /// Instantiates <see cref="SQLDataRowProvider"/>. Used for submission of queries with SQL.
        /// </summary>
        /// <param name="connectionString">Connection string for use by connection.</param>
        public SQLDataRowProvider(string connectionString) {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Instantiates <see cref="SQLDataRowProvider"/>. Used for submission of queries with SQL.
        /// </summary>
        /// <param name="builder">Connection string builder which produces connection string for use by connection.</param>
        public SQLDataRowProvider(SqlConnectionStringBuilder builder) : this(builder.ToString()) { }

        /// <summary>
        /// Connection timeout in seconds.
        /// </summary>
        public int Timeout { get; set; } = 30;

        /// <summary>
        /// Performs submission of query to receive data rows;
        /// </summary>
        /// <param name="sql">Query text.</param>
        /// <param name="commandType">Type of command.</param>
        /// <param name="parameters">Parameters for query.</param>
        /// <returns>An <see cref="OperationResult"/> containing a <see cref="List{T}"/> of <see cref="DataRow"/>.</returns>
        public OperationResult<List<DataRow>> SubmitQuery(string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters) =>
            PerformDataBaseCall(sql, commandType, parameters, command => {
                DataTable result = new DataTable();
                using (var reader = command.ExecuteReader())
                    result.Load(reader);
                return result.AsEnumerable().ToList();
            });

        /// <summary>
        /// Performs submission of query to receive datasets.
        /// </summary>
        /// <param name="sql">Query text.</param>
        /// <param name="commandType">Type of command.</param>
        /// <param name="parameters">Parameters for query.</param>
        /// <returns>An <see cref="OperationResult"/> containing a <see cref="DataSet"/>.</returns>
        public OperationResult<DataSet> SubmitQueryForDataSet(string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters) =>
            PerformDataBaseCall(sql, commandType, parameters, command => {
                var dataSet = new DataSet();
                var dataAdapter = new SqlDataAdapter(command);
                dataAdapter.Fill(dataSet);
                return dataSet;
            });

        /// <summary>
        /// Method to perform database call.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="sql">Query text.</param>
        /// <param name="commandType">Type of command.</param>
        /// <param name="parameters">Parameters for query.</param>
        /// <param name="callback">Method to use command to produce output.</param>
        /// <returns>An <see cref="OperationResult"/> containing <typeparamref name="T"/>.</returns>
        protected OperationResult<T> PerformDataBaseCall<T>(string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters, Func<SqlCommand, T> callback) =>
            Get.OperationResult(() => {
                parameters = parameters ?? new KeyValuePair<string, object>[0];
                var command = new SqlCommand {
                    CommandTimeout = Timeout,
                    CommandText = sql,
                    CommandType = commandType
                };
                command.Parameters.AddRange(
                    parameters.Select(p => {

                        var parameter = new SqlParameter(p.Key, p.Value ?? DBNull.Value);
                        if (p.Value is string)
                            parameter.DbType = DbType.AnsiString;
                        else if (p.Value is System.Xml.Linq.XDocument) {
                            parameter.DbType = DbType.Xml;
                            parameter.Value = new SqlXml(((System.Xml.Linq.XDocument)p.Value).CreateReader());
                        }

                        return parameter;
                    }).ToArray()
                );

                using (var connection = new SqlConnection(_connectionString)) {
                    command.Connection = connection;
                    connection.Open();

                    return callback(command);
                }
            });

    }
}