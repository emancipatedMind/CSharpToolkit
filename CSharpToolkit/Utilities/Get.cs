namespace CSharpToolkit.Utilities {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Mail;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Logging;
    using DataAccess;
    using DataAccess.Abstractions;

    /// <summary>
    /// Utility class which is used to obtain some kind of object.
    /// </summary>
    public static class Get {

        /*
        public static OperationResult<T> EnumValue<T>(object v) {
            try {
                if (Enum.IsDefined(typeof(T), v)) {
                    if (v.GetType() == typeof(string))
                        return new OperationResult<T>((T)Enum.Parse(typeof(T), v.ToString(), true));

                    return new OperationResult<T>((T)v);
                }
                return new OperationResult<T>(false, default(T));
            }
            catch (Exception ex) {
                return new OperationResult<T>(new[] { ex });
            }
        }
        */

        /// <summary>
        /// Used to get a new guid where the dashes are replaced by underscores.
        /// </summary>
        /// <returns>A new guid where the dashes are replaced by underscores.</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static string SafeGuid() =>
            Guid.NewGuid().ToString().Replace("-", "_");

        /// <summary>
        /// An exception-free way to get a substring. If the input string is shorter than length - startIndex, the startIndex character to end of input string is returned. If null, null is returned.
        /// </summary>
        /// <param name="input">The string to substring.</param>
        /// <param name="startIndex">Where to start substring.</param>
        /// <param name="length">Substring length.</param>
        /// <returns>Substring requested.</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static string Substring(string input, int startIndex, int length) {
            if (string.IsNullOrEmpty(input))
                return input;

            if (length == 0 || startIndex > input.Length)
                return "";

            return input.Substring(startIndex, Math.Min(input.Length - startIndex, length));
        }

        /// <summary>
        /// Gets callback method for writing out to file.
        /// </summary>
        /// <param name="fileName">File name to write out to.</param>
        /// <returns>Callback method for writing out to file.</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static Action<string> WriteMethod(string fileName) =>
            WriteMethod(new FileInfo(fileName));

        /// <summary>
        /// Gets callback method for writing out to file.
        /// </summary>
        /// <param name="file">File to write out to.</param>
        /// <returns>Callback method for writing out to file.</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static Action<string> WriteMethod(FileInfo file) =>
            s => { using (var stream = file.AppendText()) stream.Write(s); };

        [System.Diagnostics.DebuggerStepThrough]
        public static OperationResult<DateTime?> BuildDate(this Assembly assembly) =>
            OperationResult<DateTime?>(() => {
                const int c_PeHeaderOffset = 60;
                const int c_LinkerTimestampOffset = 8;
                
                string filePath = assembly.Location;
                byte[] buffer = new byte[2048];
                TimeZoneInfo target = TimeZoneInfo.Local;

                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    stream.Read(buffer, 0, 2048);

                int offset = BitConverter.ToInt32(buffer, c_PeHeaderOffset);
                int secondsSince1970 = BitConverter.ToInt32(buffer, offset + c_LinkerTimestampOffset);
                DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                DateTime linkTimeUtc = epoch.AddSeconds(secondsSince1970);

                return TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, target);
            });

        /// <summary>
        /// Gets list of type T.
        /// </summary>
        /// <typeparam name="T">List type.</typeparam>
        /// <param name="action">Operation to fill list before returning.</param>
        /// <returns>Returns new list.</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static List<T> List<T>(Action<List<T>> action) =>
            General(action);

        /// <summary>
        /// Gets generic object of type T.
        /// </summary>
        /// <typeparam name="T">object type.</typeparam>
        /// <param name="acton">Operation to perform on object before returning.</param>
        /// <param name="parameters">Parameters to be passed to object of type T for creating.</param>
        /// <returns>Returns new object.</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static T General<T>(Action<T> action, object[] parameters = null) {
            var g = (T)Activator.CreateInstance(typeof(T), parameters);
            action(g);
            return g;
        }

        /// <summary>
        /// Gets operation result. Operation must return bool, and this denotes whether operation was successful or not. Any exceptions thrown are captured, and returned as OperationResult.Exceptions.
        /// </summary>
        /// <param name="operation">Operation to perform.</param>
        /// <returns>Operation result.</returns>
        public static OperationResult OperationResult(Func<bool> operation) {
            try {
                return new OperationResult(operation());
            }
            catch (Exception ex) {
                return new OperationResult(new Exception[] { ex });
            }
        }

        /// <summary>
        /// Gets operation result. This is used when operation just needs to complete without error to be deemed a success.
        /// </summary>
        /// <param name="operation">Operation to complete.</param>
        /// <returns>Operation result.</returns>
        public static OperationResult OperationResult(Action operation) {
            try {
                operation();
                return new OperationResult();
            }
            catch (Exception ex) {
                return new OperationResult(new Exception[] { ex });
            }
        }

        /// <summary>
        /// Gets operation result. This is used when operation just needs to complete without error to be deemed a success. 
        /// </summary>
        /// <param name="operation">Operation to complete.</param>
        /// <returns>Operation result.</returns>
        public static OperationResult OperationResult(Func<OperationResult> operation) {
            try {
                return operation();
            }
            catch (Exception ex) {
                return new OperationResult(new Exception[] { ex });
            }
        }

        /// <summary>
        /// Gets operation result. Uses return value of operation which becomes available through OperationResult.Result.
        /// </summary>
        /// <param name="operation">Operation to complete.</param>
        /// <returns>Operation result.</returns>
        public static OperationResult<T> OperationResult<T>(Func<T> operation) {
            try {
                return new OperationResult<T>(operation());
            }
            catch (Exception ex) {
                return new OperationResult<T>(new Exception[] { ex });
            }
        }

        /// <summary>
        /// Gets operation result with the async await pattern. Operation must return bool, and this denotes whether operation was successful or not. Any exceptions thrown are captured, and returned as OperationResult.Exceptions.
        /// </summary>
        /// <param name="operation">Operation to complete.</param>
        /// <returns>Operation result.</returns>
        public async static Task<OperationResult> OperationResultAsync(Func<bool> operation) =>
            await Task.Run(() => {
                try {
                    return new OperationResult(operation());
                }
                catch (Exception ex) {
                    return new OperationResult(new Exception[] { ex });
                }
            });

        /// <summary>
        /// Gets operation result with the async await pattern. This is used when operation just needs to complete without error to be deemed a success.
        /// </summary>
        /// <param name="operation">Operation to complete.</param>
        /// <returns>Operation result.</returns>
        public async static Task<OperationResult> OperationResultAsync(Action operation) =>
            await Task.Run(() => {
                try {
                    operation();
                    return new OperationResult();
                }
                catch (Exception ex) {
                    return new OperationResult(new Exception[] { ex });
                }
            });


        /// <summary>
        /// Gets operation result with the async await pattern. This is used when operation just needs to complete without error to be deemed a success. 
        /// </summary>
        /// <param name="operation">Operation to complete.</param>
        /// <returns>Operation result.</returns>
        public async static Task<OperationResult> OperationResultAsync(Func<OperationResult> operation) =>
            await Task.Run(() => {
                try {
                    return new OperationResult(operation());
                }
                catch (Exception ex) {
                    return new OperationResult(new Exception[] { ex });
                }
            });

        /// <summary>
        /// Gets operation result with the async await pattern. Uses return value of operation which becomes available through OperationResult.Result.
        /// </summary>
        /// <param name="operation">Operation to complete.</param>
        /// <returns>Operation result.</returns>
        public async static Task<OperationResult<T>> OperationResultAsync<T>(Func<T> operation) =>
            await Task.Run(() => {
                try {
                    return new OperationResult<T>(operation());
                }
                catch (Exception ex) {
                    return new OperationResult<T>(new Exception[] { ex });
                }
            });

        /// <summary>
        /// Creates a simple update query.
        /// </summary>
        /// <param name="tableName">The name of the table to be updated.</param>
        /// <param name="columns">A collection of KeyValuePairs that represents the column names with associated value.</param>
        /// <param name="whereCondition">The where condition which denotes the rows to update.</param>
        /// <returns>A data order containing the query, parameters, and aliases.</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static IAliasedDataOrder UpdateQuery(string tableName, IEnumerable<KeyValuePair<string, object>> columns, Clause whereCondition) =>
            UpdateQuery(tableName, columns, whereCondition, new string[0]);

        /// <summary>
        /// Creates a simple update query with optional output parameters.
        /// </summary>
        /// <param name="tableName">The name of the table to be updated.</param>
        /// <param name="columns">A collection of KeyValuePairs that represents the column names with associated value.</param>
        /// <param name="whereCondition">The where condition which denotes the rows to update.</param>
        /// <param name="outputParameters">The columns for which output parameters are requested. The output values are aliased. The old values are preceded with DELETED, and the new values are preceded with INSERTED. If the Name column is requested, the old value is aliased as DELETEDName, and the new value is aliased as INSERTEDName.</param>
        /// <returns>A data order containing the query, parameters, and aliases.</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static IAliasedDataOrder UpdateQuery(string tableName, IEnumerable<KeyValuePair<string, object>> columns, Clause whereCondition, IEnumerable<string> outputParameters) {
            var outputClauseFunctions = new Func<string, IEnumerable<string>, IAliasedDataOrder>[] {
                ProduceInsertedOutputClause,
                ProduceDeletedOutputClause,
            };

            Func<IEnumerable<KeyValuePair<string, string>>, string, string> queryProducer =
                (columnsWithParameterNames, outputClause) =>
                    $"UPDATE {tableName} SET"
                    + $"\r\n{string.Join(",\r\n", columnsWithParameterNames.Select(c => $"    {c.Key} = {c.Value}"))}"
                    + outputClause;

            return CreateQuery(tableName, columns, whereCondition, outputParameters, queryProducer, outputClauseFunctions);
        }

        /// <summary>
        /// Creates a simple insert query. Only default values will be inserted.
        /// </summary>
        /// <param name="tableName">The name of the table to be updated.</param>
        /// <returns>A data order containing the query, parameters, and aliases.</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static IAliasedDataOrder InsertQuery(string tableName) =>
            InsertQuery(tableName, new KeyValuePair<string, object>[0], new string[0]);

        /// <summary>
        /// Creates a simple insert query with optional output parameters.
        /// </summary>
        /// <param name="tableName">The name of the table to be updated.</param>
        /// <param name="columns">A collection of KeyValuePairs that represents the column names with associated value.</param>
        /// <returns>A data order containing the query, parameters, and aliases.</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static IAliasedDataOrder InsertQuery(string tableName, IEnumerable<KeyValuePair<string, object>> columns) =>
            InsertQuery(tableName, columns, new string[0]);

        /// <summary>
        /// Creates a simple insert query with optional output parameters.
        /// </summary>
        /// <param name="tableName">The name of the table to be updated.</param>
        /// <param name="outputParameters">The columns for which output parameters are requested. The output values are aliased. The column names are preceded with INSERTED. For example, an output of the column Id would be INSERTEDId.</param>
        /// <returns>A data order containing the query, parameters, and aliases.</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static IAliasedDataOrder InsertQuery(string tableName, IEnumerable<string> outputParameters) =>
            InsertQuery(tableName, new KeyValuePair<string, object>[0], outputParameters);

        /// <summary>
        /// Creates a simple insert query with optional output parameters.
        /// </summary>
        /// <param name="tableName">The name of the table to be updated.</param>
        /// <param name="columns">A collection of KeyValuePairs that represents the column names with associated value.</param>
        /// <param name="outputParameters">The columns for which output parameters are requested. The output values are aliased. The column names are preceded with INSERTED. For example, an output of the column Id would be INSERTEDId.</param>
        /// <returns>A data order containing the query, parameters, and aliases.</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static IAliasedDataOrder InsertQuery(string tableName, IEnumerable<KeyValuePair<string, object>> columns, IEnumerable<string> outputParameters) {
            var outputClauseFunctions = new Func<string, IEnumerable<string>, IAliasedDataOrder>[] {
                ProduceInsertedOutputClause
            };

            Func<IEnumerable<KeyValuePair<string, string>>, string, string> queryProducer =
                (columnsWithParameterNames, outputClause) =>
                    columnsWithParameterNames.Any() ?
                        $"INSERT INTO {tableName} ("
                            + "\r\n"
                            + string.Join(",\r\n", columnsWithParameterNames.Select(c => $"    {c.Key}"))
                            + $"\r\n)"
                            + outputClause
                            + "\r\nVALUES ("
                            + "\r\n"
                            + string.Join(",\r\n", columnsWithParameterNames.Select(c => $"    {c.Value}"))
                            + $"\r\n)" :
                        $"INSERT INTO {tableName}"
                        + $"{outputClause}\r\n"
                        + "DEFAULT VALUES";

            return CreateQuery(tableName, columns, Clause.New(ClauseType.AND), outputParameters, queryProducer, outputClauseFunctions);
        }

        /// <summary>
        /// Creates a simple delete query with optional output parameters.
        /// </summary>
        /// <param name="tableName">The name of the table to be updated.</param>
        /// <param name="whereCondition">The where condition which denotes the rows to update.</param>
        /// <returns>A data order containing the query, parameters, and aliases.</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static IAliasedDataOrder DeleteQuery(string tableName, Clause whereCondition) =>
            DeleteQuery(tableName, whereCondition, new string[0]);

        /// <summary>
        /// Creates a simple delete query with optional output parameters.
        /// </summary>
        /// <param name="tableName">The name of the table to be updated.</param>
        /// <param name="whereCondition">The where condition which denotes the rows to update.</param>
        /// <param name="outputParameters">The columns for which output parameters are requested. The output values are aliased. The column names are preceded with DELETED. For example, an output of the column Id would be DELETEDId.</param>
        /// <returns>A data order containing the query, parameters, and aliases.</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static IAliasedDataOrder DeleteQuery(string tableName, Clause whereCondition, IEnumerable<string> outputParameters) {
            var outputClauseFunctions = new Func<string, IEnumerable<string>, IAliasedDataOrder>[] {
                ProduceDeletedOutputClause,
            };

            Func<IEnumerable<KeyValuePair<string, string>>, string, string> queryProducer =
                (columnsWithParameterNames, outputClause) =>
                    $"DELETE FROM {tableName}"
                    + outputClause;

            return CreateQuery(tableName, new KeyValuePair<string, object>[0], whereCondition, outputParameters, queryProducer, outputClauseFunctions);
        }


        private static IAliasedDataOrder CreateQuery(string tableName,
            IEnumerable<KeyValuePair<string, object>> columns,
            Clause whereCondition,
            IEnumerable<string> outputParameters,
            Func<IEnumerable<KeyValuePair<string, string>>, string, string> queryProducer,
            IEnumerable<Func<string, IEnumerable<string>, IAliasedDataOrder>> outputClauseFunctions
            ) {

            var columnArray = columns.ToArray();

            var columnsWithParameterNames = new List<KeyValuePair<string, string>>();
            var parameterNamesWithValue = new List<KeyValuePair<string, object>>();
            for (int i = 0; i < columnArray.Length; i++) {
                KeyValuePair<string, object> pair = columnArray[i];
                string parameterName = Clause.GenerateParameterName();
                columnsWithParameterNames.Add(new KeyValuePair<string, string>(pair.Key, parameterName));
                parameterNamesWithValue.Add(new KeyValuePair<string, object>(parameterName, pair.Value));
            }

            string outputClause = "";
            List<IAliasedDataOrder> outputDataOrders = new List<IAliasedDataOrder>();
            if (outputParameters.Any()) {
                outputDataOrders.AddRange(outputClauseFunctions.Select(f => f(tableName, outputParameters)));

                if (outputDataOrders.Any()) {
                    outputClause = "\r\nOUTPUT"
                        + $"\r\n{string.Join(",\r\n", outputDataOrders.Select(dO => dO.Query))}";
                }
            }

            ISimpleDataOrder whereClause = whereCondition.Build();

            var parameters =
                Get.List<KeyValuePair<string, object>>(list => {
                    list.AddRange(parameterNamesWithValue);
                    list.AddRange(whereClause.Parameters);
                });

            string query = queryProducer(columnsWithParameterNames, outputClause);
            if (string.IsNullOrEmpty(whereClause.Query) == false) {
                query += "\r\nWHERE" + whereClause.Query;
            }

            return new AliasedDataOrder(query, parameters, outputDataOrders.SelectMany(dO => dO.Aliases));
        }

        private static IAliasedDataOrder ProduceDeletedOutputClause(string table, IEnumerable<string> parameters) =>
            ProduceOutputClause(table, parameters, "DELETED");
        private static IAliasedDataOrder ProduceInsertedOutputClause(string table, IEnumerable<string> parameters) =>
            ProduceOutputClause(table, parameters, "INSERTED");

        private static IAliasedDataOrder ProduceOutputClause(string table, IEnumerable<string> parameters, string keyword) {
            string prefix = keyword;
            var query = string.Join(",\r\n", parameters.Select(p => $"    {keyword}.{p} AS {prefix}{p}"));
            List<Alias> aliases =
                parameters.Select(p => new Alias($"{prefix}{table}", new KeyValuePair<string, string>(p, $"{prefix}{p}"))).ToList();
            return new AliasedDataOrder(query, aliases);
        }

        /// <summary>
        /// Gets PropertyInfo of optionally embedded property.
        /// </summary>
        /// <param name="target">Target of query.</param>
        /// <param name="compoundProperty">Optionally embedded property info to get. If you have an object with a property, just specify that property like so "Name". If property has property, syntax is "Name.Key". Syntax may be repeated until property is discovered.</param>
        /// <returns>Operation result containing propertyinfo, and target object propertyinfo was found on.</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static OperationResult<Tuple<object, PropertyInfo>> Property(object target, string compoundProperty) =>
            OperationResult(() => {
                object originalTarget = target;
                string[] bits = compoundProperty.Split('.');
                BindingFlags flags =
                    BindingFlags.NonPublic |
                    BindingFlags.Public |
                    BindingFlags.Instance |
                    BindingFlags.GetProperty |
                    BindingFlags.SetProperty;

                for (int i = 0; i < bits.Length - 1; i++) {
                    PropertyInfo propertyToGet = target.GetType().GetProperty(bits[i], flags);
                    target = propertyToGet.GetValue(target, null);
                }
                var propertyInfoToReturn = target.GetType().GetProperty(bits.Last(), flags);
                if (propertyInfoToReturn == null)
                    throw new ArgumentException($"On {originalTarget}, {compoundProperty} could not be found.");
                return Tuple.Create(target, propertyInfoToReturn);
            });

        /// <summary>
        /// Gets a query that can be used to copy records only changing a few fields.
        /// </summary>
        /// <param name="tableName">The name of the table in which the duplication will occur.</param>
        /// <param name="tempTableClause">The clause that will used to filter the table.</param>
        /// <param name="primaryKeyFieldName">The name of the primaryKey field.</param>
        /// <param name="fieldParameterNameTupleCollection">The collection of Tuples to not be copied, but replaced. Item1 should be the field of the table, and Item2 should be the parameter name of the new value.</param>
        /// <param name="fieldsToCopy">The fields that should be copied verbatim.</param>
        /// <returns>A simple data order containing the parameters from the Clause, and the resultant query. The resultant query outputs the primary key of the inserted record as NewId, and the old Id as OldId.</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static ISimpleDataOrder RecordCopyQuery(string tableName, string primaryKeyFieldName, Clause tempTableClause, Tuple<string, string>[] fieldParameterNameTupleCollection, string[] fieldsToCopy) {
            string uniqueAppendage = SafeGuid();
            string sourceAlias = "source";

            string query = $"MERGE {tableName}\r\nUSING ("
            + $"\r\nSELECT{string.Join(",", fieldsToCopy.Concat(new[] { primaryKeyFieldName }).Distinct().Select(field => $"\r\n   {field}"))}\r\nFROM {tableName}";

            SimpleDataOrder firstDataOrder =
                tempTableClause
                    .Build();

            if (string.IsNullOrEmpty(firstDataOrder.Query) == false) {
                query += "\r\nWHERE" + firstDataOrder.Query;
            }
            query += $"\r\n) AS {sourceAlias}\r\nON (1 = 0)\r\nWHEN NOT MATCHED THEN";

            IEnumerable<Tuple<string, string>> insertValues = fieldsToCopy.Select(field => Tuple.Create(field, $"{sourceAlias}.{field}")).Concat(fieldParameterNameTupleCollection);

            query += $"\r\nINSERT ({string.Join(",", insertValues.Select(tuple => $"\r\n    {tuple.Item1}"))}\r\n)\r\nVALUES (\r\n";
            query += string.Join(",", insertValues.Select(tuple => $"\r\n    {tuple.Item2}"));
            query += $"\r\n)\r\nOUTPUT {sourceAlias}.{primaryKeyFieldName} AS OldId, INSERTED.{primaryKeyFieldName} AS NewId;";

            return new SimpleDataOrder(query, firstDataOrder.Parameters);
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static OperationResult<MailMessage> MailMessage(string subject, string body, EmailReceipients receipients, MailAddress sender, bool isBodyHtml) {
            var exceptions = new List<Exception>();
            if (receipients == null || receipients.NoReceipientsSpecified)
                exceptions.Add(new ArgumentException("No receipients for email were specified."));
            if (string.IsNullOrWhiteSpace(subject) && string.IsNullOrWhiteSpace(body))
                exceptions.Add(new ArgumentException("An email cannot be sent with no content. Both subject and body are empty."));
            if (sender == null)
                exceptions.Add(new ArgumentException("A sender must be specified."));

            if (exceptions.Any())
                return new OperationResult<MailMessage>(exceptions);

            MailMessage message = receipients.GetReceipientHydratedMailMessage();
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = isBodyHtml;
            message.Sender = sender;

            return new OperationResult<MailMessage>(message);
        }

        /// <summary>
        /// An instance of the <see cref="Regex"/> class that will validate emails.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static Regex EmailRegex() =>
                new Regex(
                    @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                    + "@"
                    + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$"
                );
    }
}