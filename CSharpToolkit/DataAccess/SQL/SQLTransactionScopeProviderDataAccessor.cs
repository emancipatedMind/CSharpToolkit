namespace CSharpToolkit.DataAccess.SQL {
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Abstractions;
    using Utilities;

    public class SQLTransactionScopeProviderDataAccessor : ITransactionScopeProviderDataAccessor {

        string _connectionString;
        IDataAccessor _dataAccessor;
        Func<ITransactionScopeDataAccessor> _callback;

        public SQLTransactionScopeProviderDataAccessor(System.Data.SqlClient.SqlConnectionStringBuilder stringBuilder) : this(stringBuilder.ToString()) { }
        public SQLTransactionScopeProviderDataAccessor(string connectionString) : this(new SQLDataAccessor(connectionString), () => new SQLTransactionScopeDataAccessor(connectionString)) { }
        public SQLTransactionScopeProviderDataAccessor(IDataAccessor dataAccessor, Func<ITransactionScopeDataAccessor> callback) {
            _dataAccessor = dataAccessor;
            _callback = callback;
        }

        public ITransactionScopeDataAccessor GetTransactionScopeDataAccessor() =>
            _callback();

        public OperationResult<int> PerformDataOperation(string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters) =>
            _dataAccessor.PerformDataOperation(sql, commandType, parameters);

        public OperationResult<List<DataRow>> SubmitQuery(string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters) =>
            _dataAccessor.SubmitQuery(sql, commandType, parameters);

        public OperationResult<DataSet> SubmitQueryForDataSet(string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters) =>
            _dataAccessor.SubmitQueryForDataSet(sql, commandType, parameters);

        public OperationResult<object> SubmitScalarQuery(string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters) =>
            _dataAccessor.SubmitScalarQuery(sql, commandType, parameters);


    }
}
