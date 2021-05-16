namespace CSharpToolkit.Logging {
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Abstractions;
    using DataAccess;
    using DataAccess.Abstractions;
    using Utilities;
    using Utilities.Abstractions;
    using Utilities.Formatters;
    using Extensions;

    /// <summary>
    /// Used as a Decorator for the ITransactionScopeDataAccessor that will log requests as valid T-SQL.
    /// </summary>
    public class SQLTransactionScopeLoggingDataAccessor : ITransactionScopeDataAccessor {

        static TSqlFormatter DefaultDataOrderFormatter = new TSqlFormatter();
        static ExceptionFormatter DefaultExceptionFormatter = new ExceptionFormatter();

        IExceptionFormatter _formatter;
        ILogger _logger;
        ITransactionScopeDataAccessor _component;
        IAliasedCommandTypeDataOrderFormatter _dataOrderformatter;
        string _scopeGUID;

        /// <summary>
        /// Instantiates SQLLoggingDataAccessor.
        /// </summary>
        /// <param name="component">The object implementing ITransactionScopeDataAccessor being wrapped.</param>
        /// <param name="fileName">The filename for the log. Will log with timestamp.</param>
        public SQLTransactionScopeLoggingDataAccessor(ITransactionScopeDataAccessor component, string fileName) : this(component, new TimeStampLogger(fileName) { LinePrependageText = "\r\n" }) { }

        /// <summary>
        /// Instantiates SQLLoggingDataAccessor.
        /// </summary>
        /// <param name="component">The object implementing ITransactionScopeDataAccessor being wrapped.</param>
        /// <param name="logger">The logger to be used.</param>
        public SQLTransactionScopeLoggingDataAccessor(ITransactionScopeDataAccessor component, ILogger logger) {
            _component = component;
            _logger = logger;
        }

        /// <summary>
        /// Formatter to use for exceptions found.
        /// </summary>
        public IExceptionFormatter ExceptionFormatter {
            get { return _formatter ?? DefaultExceptionFormatter; }
            set { _formatter = value; }
        } 

        /// <summary>
        /// Formatter to use for incoming data order.
        /// </summary>
        public IAliasedCommandTypeDataOrderFormatter DataOrderFormatter {
            get { return _dataOrderformatter ?? DefaultDataOrderFormatter; }
            set { _dataOrderformatter = value; }
        } 

        /// <summary>
        /// Performs submission of query. Operation will be logged as valid T-SQL.
        /// </summary>
        /// <param name="sql">Query text.</param>
        /// <param name="commandType">Type of command.</param>
        /// <param name="parameters">Parameters for query.</param>
        /// <returns>Operation result with a list of DataRows.</returns>
        public OperationResult<List<DataRow>> SubmitQuery(string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters) =>
            ExecuteWrapper(sql, commandType, parameters, _component.SubmitQuery);

        /// <summary>
        /// Performs submission of query to return a single value. Operation will be logged as valid T-SQL.
        /// </summary>
        /// <param name="sql">Query text.</param>
        /// <param name="commandType">Type of command.</param>
        /// <param name="parameters">Parameters for query.</param>
        /// <returns>Operation result containing the first field of the first row, or a single value.</returns>
        public OperationResult<object> SubmitScalarQuery(string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters) =>
            ExecuteWrapper(sql, commandType, parameters, _component.SubmitScalarQuery);

        /// <summary>
        /// Performs submission of query to receive datasets.  Operation will be logged as valid T-SQL.
        /// </summary>
        /// <param name="sql">Query text.</param>
        /// <param name="commandType">Type of command.</param>
        /// <param name="parameters">Parameters for query.</param>
        /// <returns>An <see cref="OperationResult"/> containing a <see cref="DataSet"/>.</returns>
        public OperationResult<DataSet> SubmitQueryForDataSet(string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters) =>
            ExecuteWrapper(sql, commandType, parameters, _component.SubmitQueryForDataSet);

        /// <summary>
        /// Performs a data operation that returns number of rows affected. Operation will be logged as valid T-SQL.
        /// </summary>
        /// <param name="sql">Query text.</param>
        /// <param name="commandType">Type of command.</param>
        /// <param name="parameters">Parameters for query.</param>
        /// <returns>Operation result containing number of rows affected.</returns>
        public OperationResult<int> PerformDataOperation(string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters) =>
            ExecuteWrapper(sql, commandType, parameters, _component.PerformDataOperation);

        private OperationResult<T> ExecuteWrapper<T>(string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters, Func<string, CommandType, IEnumerable<KeyValuePair<string, object>>, OperationResult<T>> callback) {
            var operation = callback(sql, commandType, parameters);

            System.Threading.Tasks.Task.Run(() =>
                Use.StringBuilder(builder => {
                    if (_scopeGUID.IsMeaningful() == false) {
                        _scopeGUID = Get.SafeGuid();
                        builder.AppendLine($"TX - Transaction reference number {_scopeGUID} created.");
                        builder.AppendLine();
                        builder.AppendLine(new string('*', 120));
                    }
                    builder.AppendLine();
                    builder.Append(DataOrderFormatter.Format(new AliasedCommandTypeDataOrder(sql, commandType, parameters)));
                    builder.AppendLine();

                    if (operation.WasSuccessful) {
                        builder.AppendLine("Operation completed successfully.");
                    }
                    else {
                        builder.AppendLine("Operation was unsuccessful.");
                        if (operation.HadErrors) {
                            builder.AppendLine();
                            builder.AppendLine("Operation was faulted.");

                            builder.AppendLine();
                            builder.AppendLine(DefaultExceptionFormatter.FormatException(operation.Exceptions));
                        }
                    }

                    builder.AppendLine();
                    builder.AppendLine(new string('*', 120));
                    _logger.Log(builder);
                })
            );

            return operation;
        }

        public OperationResult CommitTransaction() {
            var operation = _component.CommitTransaction();
            _logger.Log("\r\n");
            if (operation.WasSuccessful) {
                _logger.Log("Transaction successfully committed.");
            }
            else {
                _logger.Log("Transaction committal failed.");
            }
            _logger.Log("\r\n");
            _logger.Log($"TX - Transaction reference number {_scopeGUID} destroyed.");
            _logger.Log("\r\n");
            _logger.Log("\r\n");
            _logger.Log(new string('*', 120));
            _logger.Log("\r\n");
            return operation;
        }

        public OperationResult RollbackTransaction() {
            var operation = _component.RollbackTransaction();
            _logger.Log("\r\n");
            if (operation.WasSuccessful) {
                _logger.Log("Transaction successfully rolled back.");
            }
            else {
                _logger.Log("Transaction rollback failed.");
            }
            _logger.Log("\r\n");
            _logger.Log($"TX - Transaction reference number {_scopeGUID} destroyed.");
            _logger.Log("\r\n");
            _logger.Log("\r\n");
            _logger.Log(new string('*', 120));
            _logger.Log("\r\n");
            return operation;
        }
    }
}
