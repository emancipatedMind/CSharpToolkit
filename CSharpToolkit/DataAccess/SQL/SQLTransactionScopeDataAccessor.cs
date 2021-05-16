namespace CSharpToolkit.DataAccess.SQL {
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Xml.Linq;
    using Utilities;

    public class SQLTransactionScopeDataAccessor : Abstractions.ITransactionScopeDataAccessor {

        System.Transactions.CommittableTransaction _commitableTransaction;
        System.Data.SqlClient.SqlConnection _connection;
        string _connectionString;
        bool _disposed;

        public SQLTransactionScopeDataAccessor(SqlConnectionStringBuilder connectionString) : this(connectionString.ToString()) {
            _connectionString = connectionString.ToString();
        }

        public SQLTransactionScopeDataAccessor(string connectionString) {
            _connectionString = connectionString;
        }

        public int Timeout { get; set; } = 30;

        public OperationResult CommitTransaction() =>
            Get.OperationResult(() => {
                FinishOperation(() => _commitableTransaction?.Commit());
            });

        public OperationResult RollbackTransaction() =>
            Get.OperationResult(() => {
                FinishOperation(() => _commitableTransaction?.Rollback());
            });

        public OperationResult<List<DataRow>> SubmitQuery(string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters) =>
            PerformDataBaseCall(sql, commandType, parameters, command => {
                System.Data.DataTable result = new System.Data.DataTable();
                using (var reader = command.ExecuteReader())
                    result.Load(reader);
                return result.AsEnumerable().ToList();
            });

        public OperationResult<DataSet> SubmitQueryForDataSet(string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters) =>
            PerformDataBaseCall(sql, commandType, parameters, command => {
                var dataSet = new DataSet();
                var dataAdapter = new SqlDataAdapter(command);
                dataAdapter.Fill(dataSet);
                return dataSet;
            });

        public OperationResult<int> PerformDataOperation(string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters) =>
            PerformDataBaseCall(sql, commandType, parameters, command => command.ExecuteNonQuery());

        public OperationResult<object> SubmitScalarQuery(string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters) =>
            PerformDataBaseCall(sql, commandType, parameters, command => command.ExecuteScalar());

        protected OperationResult<T> PerformDataBaseCall<T>(string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters, Func<SqlCommand, T> callback) =>
            Get.OperationResult(() => {
                try {
                    if (_disposed) {
                        throw new System.ObjectDisposedException("This transaction scope has already been disposed.");
                    }

                    if (_connection == null) {
                        _commitableTransaction = new System.Transactions.CommittableTransaction();
                        _connection = new SqlConnection(_connectionString.ToString());
                        _connection.Open();
                        _connection.EnlistTransaction(_commitableTransaction);
                    }

                    parameters = parameters ?? new KeyValuePair<string, object>[0];
                    var command = new SqlCommand {
                        Connection = _connection,
                        CommandTimeout = Timeout,
                        CommandText = sql,
                        CommandType = commandType
                    };
                    command.Parameters.AddRange(
                        parameters.Select(p => {

                            var parameter = new SqlParameter(p.Key, p.Value ?? DBNull.Value);
                            if (p.Value is string)
                                parameter.DbType = DbType.AnsiString;
                            else if (p.Value is XDocument) {
                                parameter.DbType = DbType.Xml;
                                parameter.Value = new System.Data.SqlTypes.SqlXml(((XDocument)p.Value).CreateReader());
                            }

                            return parameter;
                        }).ToArray()
                    );

                    return callback(command);
                }
                catch {
                    FinishOperation(() => _commitableTransaction?.Rollback());
                    throw;
                }
            });

        void FinishOperation(System.Action callback) {
            try {
                callback();
            }
            catch {
                _commitableTransaction?.Rollback();
                throw;
            }
            finally {
                _connection?.Close();
                _disposed = true;
            }
        }

    }
}
