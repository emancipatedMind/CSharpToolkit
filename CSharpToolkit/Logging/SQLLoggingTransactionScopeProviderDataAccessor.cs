namespace CSharpToolkit.Logging {
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Abstractions;
    using Utilities;
    using System.Linq;
    using DataAccess;
    using DataAccess.Abstractions;
    using Utilities.Abstractions;
    using Utilities.Formatters;
    public class SQLLoggingTransactionScopeProviderDataAccessor : ITransactionScopeProviderDataAccessor {
        static TSqlFormatter DefaultDataOrderFormatter = new TSqlFormatter();
        static ExceptionFormatter DefaultExceptionFormatter = new ExceptionFormatter();

        IExceptionFormatter _formatter;
        ILogger _logger;
        ITransactionScopeProviderDataAccessor _component;
        IAliasedCommandTypeDataOrderFormatter _dataOrderformatter;

        /// <summary>
        /// Instantiates SQLLoggingTransactionScopeProviderDataAccessor.
        /// </summary>
        /// <param name="component">The object implementing IDataAccessor being wrapped.</param>
        /// <param name="fileName">The filename for the log. Will log with timestamp.</param>
        public SQLLoggingTransactionScopeProviderDataAccessor(ITransactionScopeProviderDataAccessor component, string fileName) : this(component, new TimeStampLogger(fileName) { LinePrependageText = "\r\n" }) { }

        /// <summary>
        /// Instantiates SQLLoggingTransactionScopeProviderDataAccessor.
        /// </summary>
        /// <param name="component">The object implementing IDataAccessor being wrapped.</param>
        /// <param name="logger">The logger to be used.</param>
        public SQLLoggingTransactionScopeProviderDataAccessor(ITransactionScopeProviderDataAccessor component, ILogger logger) {
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

        public ITransactionScopeDataAccessor GetTransactionScopeDataAccessor() =>
            new SQLTransactionScopeLoggingDataAccessor(_component.GetTransactionScopeDataAccessor(), _logger) {
                ExceptionFormatter = ExceptionFormatter,
                DataOrderFormatter = DataOrderFormatter,
            };

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

    }
}
