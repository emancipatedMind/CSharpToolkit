namespace CSharpToolkit.DataAccess.SQL {
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Abstractions;
    using Utilities;
    using System.Data.SqlClient;
    using Extensions;
    using Utilities.EventArgs;

    public class SQLServerErrorHandler : IDataAccessor, IDisposable {

        IDataAccessor _component;

        public SQLServerErrorHandler(IDataAccessor component) {
            _component = component;
        }

        public OperationResult<int> PerformDataOperation(string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters) =>
            Process(sql, commandType, parameters, _component.PerformDataOperation);

        public OperationResult<List<DataRow>> SubmitQuery(string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters) =>
            Process(sql, commandType, parameters, _component.SubmitQuery);

        public OperationResult<DataSet> SubmitQueryForDataSet(string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters) =>
            Process(sql, commandType, parameters, _component.SubmitQueryForDataSet);

        public OperationResult<object> SubmitScalarQuery(string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters) =>
            Process(sql, commandType, parameters, _component.SubmitScalarQuery);

        public List<int> CodesToRetry { get; } = new List<int> {
            1205, // DeadLock
            64, // Most Likely Killed
            -2, // Timeout
        };

        public int RetryCount { get; set; } = 3;

        public event EventHandler<GenericEventArgs<IEnumerable<SqlException>>> LogNonFatalSQLErrors;

        OperationResult<T> Process<T>(string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters, Func<string, CommandType, IEnumerable<KeyValuePair<string, object>>, OperationResult<T>> callback) {
            parameters = parameters ?? Enumerable.Empty<KeyValuePair<string, object>>();

            OperationResult<T> operation = PerformDataOperation(() => callback(sql, commandType, parameters));

            if (operation.HadErrors) {
                DateTime now = DateTime.Now;
                operation.Exceptions.Where(ex => ex is SqlException).ForEach(ex => {

                    try {
                        ex.Data.Add("Try.DateTime", now);
                    }
                    catch {
                        ex.Data.Add($"Try.DateTime_{Get.SafeGuid().SafeSubstring(0, 8)}", now);
                    }

                    try {
                        ex.Data.Add("Try.Query", sql);
                    }
                    catch {
                        ex.Data.Add($"Try.Query_{Get.SafeGuid().SafeSubstring(0, 8)}", sql);
                    }

                    try {
                        ex.Data.Add("Try.CommandType", commandType);
                    }
                    catch {
                        ex.Data.Add($"Try.CommandType_{Get.SafeGuid().SafeSubstring(0, 8)}", commandType);
                    }

                    parameters.Select((p, i) => new { p, i = i + 1 }).ForEach(x => {
                        string guid = Get.SafeGuid().SafeSubstring(0, 8);
                        try {
                            ex.Data.Add($"Try.Parameter{x.i}.Name", x.p.Key);
                        }
                        catch {
                            ex.Data.Add($"Try.Parameter{x.i}.Name_{guid}", x.p.Key);
                        }
                        try {
                            ex.Data.Add($"Try.Parameter{x.i}.Value", x.p.Value);
                        }
                        catch {
                            ex.Data.Add($"Try.Parameter{x.i}.Value_{guid}", x.p.Value);
                        }
                    });
                });

            }

            return operation;
        }

        OperationResult<T> PerformDataOperation<T>(Func<OperationResult<T>> callback) {

            int i = 1;
            List<SqlException> list = new List<SqlException>();

            while (true) {
                OperationResult<T> operation = callback();

                if (operation.HadErrors == false) {
                    if (list.Any()) {
                        LogNonFatalSQLErrors?.Invoke(this, new GenericEventArgs<IEnumerable<SqlException>>(list));
                    }

                    return operation;
                }

                operation.Exceptions.Where(ex => ex is SqlException).AttachInfo(new[] {
                    Tuple.Create<string, object>("Try.Count", i),
                });

                if (operation.HadErrors && operation.Exceptions.Any(ex => ex is SqlException == false))
                    return new OperationResult<T>(operation.WasSuccessful, operation.Result, operation.Exceptions.Concat(list));

                if (operation.Exceptions.Cast<SqlException>().All(ex => CodesToRetry.Contains(ex.Number))) {
                    if (i++ < RetryCount) {
                        list.AddRange(operation.Exceptions.Cast<SqlException>());
                        continue;
                    }
                }

                return new OperationResult<T>(operation.WasSuccessful, operation.Result, operation.Exceptions.Concat(list));
            }
        }

        public void Dispose() {
            LogNonFatalSQLErrors = null;
        }

    }
}
