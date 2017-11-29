namespace CSharpToolkit.DataAccess.SQL {
    using Abstractions.DataAccess;
    using System;
    using System.Data;
    using System.Data.SqlClient;
    public class ConnectedDataReaderAccessor : IDataReaderAccessor {

        string _connectionString;

        public ConnectedDataReaderAccessor(SqlConnectionStringBuilder connectionStringBuilder) : this(connectionStringBuilder.ToString()) { }

        public ConnectedDataReaderAccessor(string connectionString) {
            _connectionString = connectionString;
        }

        public void UseDataReader(string sql, Action<IDataReader> callback) {
            using (var connection = new SqlConnection(_connectionString)) {
                var command = new SqlCommand(sql, connection);
                connection.Open();
                using (var reader = command.ExecuteReader())
                    callback(reader);
            }
        }
    }
}