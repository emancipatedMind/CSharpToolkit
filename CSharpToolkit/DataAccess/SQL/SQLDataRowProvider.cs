namespace CSharpToolkit.DataAccess.SQL {
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Data;
    using Abstractions.DataAccess;
    using Utilities;
    using System.Data.SqlClient;

    public class SQLDataRowProvider : IDataRowProvider {

        string _connectionString;

        public SQLDataRowProvider(string connectionString) {
            _connectionString = connectionString;
        }

        public int Timeout { get; set; } = 30;

        public OperationResult<List<DataRow>> SubmitQuery(string query) =>
            SubmitQuery(query, CommandType.Text, new Dictionary<string, object>());

        OperationResult<List<DataRow>> SubmitQuery(string sql, CommandType commandType, Dictionary<string, object> parameters) =>
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