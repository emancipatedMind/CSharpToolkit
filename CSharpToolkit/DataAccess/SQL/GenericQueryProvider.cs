namespace CSharpToolkit.DataAccess {
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using CSharpToolkit.DataAccess;
    using CSharpToolkit.DataAccess.Abstractions;
    using CSharpToolkit.Extensions;
    using CSharpToolkit.Utilities;
    using DataAccess.MetaData;
    using Request;
    public static class GenericQueryProvider {

        /// <summary>
        /// Surrounds query with transaction for use by a single query.
        /// </summary>
        /// <param name="query">Query to surround transaction with.</param>
        /// <returns>Supplied query surrounded by transaction.</returns>
        public static string SurroundQueryWithTransaction(string query) =>
            Use.StringBuilder(builder => {
                string transactionParameterName = Clause.GenerateParameterName();

                builder.AppendLine($"DECLARE {transactionParameterName} int = 0;");
                builder.AppendLine("IF @@TRANCOUNT = 0");
                builder.AppendLine("BEGIN");
                builder.AppendLine("    BEGIN TRANSACTION;");
                builder.AppendLine($"    SET {transactionParameterName} = 1;");
                builder.AppendLine("END;");
                builder.AppendLine();

                builder.AppendLine("BEGIN TRY;");
                builder.AppendLine();

                builder.Append(query);

                builder.AppendLine();
                builder.AppendLine($"IF {transactionParameterName} = 1");
                builder.AppendLine("    COMMIT TRANSACTION;");
                builder.AppendLine();

                builder.AppendLine("END TRY");
                builder.AppendLine("BEGIN CATCH");
                builder.AppendLine();

                builder.AppendLine($"    IF {transactionParameterName} = 1");
                builder.AppendLine("        ROLLBACK TRANSACTION;");
                builder.AppendLine();

                builder.AppendLine("    THROW;");
                builder.AppendLine("END CATCH;");
            });

        /// <summary>
        /// Used to verify if clause condition exists in database.
        /// </summary>
        /// <param name="tableName">The table name for the verification.</param>
        /// <param name="clause">The clause to use for verification.</param>
        /// <returns><see cref="IAliasedCommandTypeDataOrder"/> containing the query, parents, and aliases.</returns>
        public static IAliasedCommandTypeDataOrder VerifyUnique(string tableName, Clause clause) {
            var clauseOrder = clause.Build();
            string sql = $"SELECT TOP 1\r\n    COUNT(*)\r\nFROM {tableName}{(clauseOrder.Query.IsMeaningful() ? $"\r\nWHERE {clauseOrder.Query}" : "") }";
            return new AliasedCommandTypeDataOrder(sql, CommandType.Text, clauseOrder.Parameters);
        }

        /// <summary>
        /// Get the record count of the clause. If the clause is empty, the table count is returned.
        /// </summary>
        /// <param name="tableName">The table name for the count. </param>
        /// <param name="clause">The clause to use to produce count.</param>
        /// <returns><see cref="IAliasedCommandTypeDataOrder"/> containing the query, parents, and aliases.</returns>
        public static IAliasedCommandTypeDataOrder GetRecordCount(string tableName, Clause clause) {
            var clauseOrder = clause.Build();
            string sql = $"SELECT\r\n    COUNT(*)\r\nFROM {tableName}{(clauseOrder.Query.IsMeaningful() ? $"\r\nWHERE {clauseOrder.Query}" : "") }";
            return new AliasedCommandTypeDataOrder(sql, CommandType.Text, clauseOrder.Parameters);
        }

        /// <summary>
        /// Produces a query to delete a record from a table.
        /// </summary>
        /// <param name="id">The id which should be deleted.</param>
        /// <param name="tableName">The name of the table which will be operated on.</param>
        /// <param name="idPropertyName">The name of the id field.</param>
        /// <returns><see cref="IAliasedCommandTypeDataOrder"/> containing the query, parents, and aliases.</returns>
        public static IAliasedCommandTypeDataOrder DeleteSingleId(int id, string tableName, string idPropertyName) {
            Clause whereClause = Clause.New().AddClause($"@id{Get.SafeGuid()}", id, Clause.EqualStatementCallback(idPropertyName));
            return new AliasedCommandTypeDataOrder(Get.DeleteQuery(tableName, whereClause), CommandType.Text);
        }

        /// <summary>
        /// Produces a query to delete multiples from a table. If no ids are passed, an empty data order is returned.
        /// </summary>
        /// <param name="ids">The ids which should be deleted.</param>
        /// <param name="tableName">The name of the table which will be operated on.</param>
        /// <param name="idPropertyName">The name of the id field.</param>
        /// <returns><see cref="IAliasedCommandTypeDataOrder"/> containing the query, parents, and aliases.</returns>
        public static IAliasedCommandTypeDataOrder DeleteIds(IEnumerable<int> ids, string tableName, string idPropertyName) {
            ids = ids?.ToArray() ?? new int[0];
            if (ids.Any() == false)
                return new AliasedCommandTypeDataOrder();

            Clause whereClause = Clause.New().AddClause(Clause.InStatement(idPropertyName, ids.Cast<object>()));
            return new AliasedCommandTypeDataOrder(Get.DeleteQuery(tableName, whereClause), CommandType.Text);
        }

        /// <summary>
        /// Used to create a query to update values on multiple tables.
        /// </summary>
        /// <param name="order">The information to create query.</param>
        /// <returns>The <see cref="IAliasedCommandTypeDataOrder"/> created.</returns>
        public static IAliasedCommandTypeDataOrder UpdateMultiple(IEnumerable<UpdateMultipleOrder> order) {
            order = order ?? new UpdateMultipleOrder[0];
            if (order.Any() == false)
                return new AliasedCommandTypeDataOrder();

            string nullKey = "@null";

            var parms =
                order
                    .SelectMany(o => o.Values)
                    .GroupBy(model => model.Item2)
                    .Where(group => group.Key != null)
                    .Select(group => Tuple.Create("@" + Get.SafeGuid(), group.Key))
                    .ToArray();

            var queries = order
                .Select(model => {

                    IEnumerable<Tuple<string, string>> updateParameters =
                        model
                            .Values
                            .Select(value => Tuple.Create(value.Item1, parms.FirstOrDefault(p => p.Item2?.Equals(value.Item2) ?? false)?.Item1 ?? nullKey));

                    string query = $"UPDATE {model.TableName} SET{string.Join(",", updateParameters.Select(tuple => $"\r\n    {tuple.Item1} = {tuple.Item2}"))}\r\nWHERE";
                    SimpleDataOrder clause = Clause.New().AddClause(model.Id.Item2, Clause.EqualStatementCallback(model.Id.Item1, model.TableName)).Build();
                    query += clause.Query;
                    return new AliasedDataOrder(query, clause.Parameters);
                }).ToArray();

            var aggregatedQuery = string.Join("\r\n\r\n", queries.Select(query => query.Query));
            var aliases = queries.SelectMany(query => query.Aliases);
            var parameters = parms.Select(tuple => new KeyValuePair<string, object>(tuple.Item1, tuple.Item2)).Concat(queries.SelectMany(query => query.Parameters)).ToList();

            if (aggregatedQuery.Contains(nullKey))
                parameters.Insert(0, new KeyValuePair<string, object>(nullKey, null));

            return new AliasedCommandTypeDataOrder(SurroundQueryWithTransaction(aggregatedQuery), System.Data.CommandType.Text, parameters, aliases);
        }

        /// <summary>
        /// Get query that can be used to copy all wanted fields into new records.
        /// </summary>
        /// <typeparam name="TModelAbstraction">The abstraction which contains metadata for the table.</typeparam>
        /// <typeparam name="TModelImplementation">The implementation of the abstraction to be used as an intermediary.</typeparam>
        /// <param name="order">The information for the query.</param>
        /// <param name="wrapInTransaction">Whether to wrap in transaction or not.</param>
        /// <returns><see cref="IAliasedCommandTypeDataOrder"/> containing the query, parents, and aliases.</returns>
        public static IAliasedCommandTypeDataOrder GetRecordCopyQuery<TModelAbstraction, TModelImplementation>(RecordCopyOrder<TModelImplementation> order, bool wrapInTransaction = true) where TModelImplementation : TModelAbstraction {

            Type abstractionType = typeof(TModelAbstraction);
            Dictionary<Type, string> metaDataDictionary = MetaDataOperations.GetMetaDataNames(abstractionType);
            string primaryKeyFieldName = metaDataDictionary
                .First(dict => dict.Key == typeof(IdAttribute))
                .Value;

            // In this case, the clause will filter out where the ParentId is the same as the current record, and then try to match any of the keys being tried for.
            Func<RecordCopyClauseCallbackOrder, Clause> clauseCallback = (rcccOrder) =>
                Clause.New()
                    .AddClause($"{order.ParentIdName} <> {rcccOrder.FieldParameterNameTupleCollection.First(tuple => tuple.Item1 == order.ParentIdName).Item2}")
                    .AddClause($"{primaryKeyFieldName} IN ({string.Join(",", rcccOrder.Keys.Select(r => $"\r\n   {r.Item1}"))}\r\n)")
                    ;

            return GetRecordCopyQuery<TModelAbstraction, TModelImplementation>(order, clauseCallback, wrapInTransaction);
        }

        /// <summary>
        /// Get query that can be used to copy all wanted fields into new records.
        /// </summary>
        /// <typeparam name="TModelAbstraction">The abstraction which contains metadata for the table.</typeparam>
        /// <typeparam name="TModelImplementation">The implementation of the abstraction to be used as an intermediary.</typeparam>
        /// <param name="order">The information for the query.</param>
        /// <param name="clauseCallback">The method that will produce the clause for the query.</param>
        /// <param name="wrapInTransaction">Whether to wrap in transaction or not.</param>
        /// <returns><see cref="IAliasedCommandTypeDataOrder"/> containing the query, parents, and aliases.</returns>
        public static IAliasedCommandTypeDataOrder GetRecordCopyQuery<TModelAbstraction, TModelImplementation>(RecordCopyOrder<TModelImplementation> order, Func<RecordCopyClauseCallbackOrder, Clause> clauseCallback, bool wrapInTransaction = true) where TModelImplementation : TModelAbstraction {
            // Ascertain types in order to use reflection.
            Type abstractionType = typeof(TModelAbstraction);
            Type implementationType = typeof(TModelImplementation);

            // This dictionary gets the meta data of the abstract type. Missing some of these will cause an exception.
            Dictionary<Type, string> metaDataDictionary = MetaDataOperations.GetMetaDataNames(abstractionType);

            // Get table name from meta data.
            string tableName = metaDataDictionary
                .First(dict => dict.Key == typeof(TableNameAttribute))
                .Value;

            // Get id set for use in filtering clause.
            KeyValuePair<string, object>[] idSet =
                order
                    .TransferKeys
                    .Select((r, index) => new KeyValuePair<string, object>(Clause.GenerateParameterName(), r))
                    .ToArray();

            // Produce a dictionary that has the values pulled in from the DefaultModel supplied through parameter.
            // Key => Name of field which should match property name.
            // Value => KeyValuePair where Key => ParameterName, Value => value of property.
            Dictionary<string, KeyValuePair<string, object>> defaultValues =
                new[] {
                    metaDataDictionary[typeof(CreatorAttribute)],
                    metaDataDictionary[typeof(DateCreatedAttribute)],
                    order.ParentIdName,
                }
                .Concat(order.DefaultFieldsToCopy)
                .Where(field => field != metaDataDictionary[typeof(IdAttribute)] && field != metaDataDictionary[typeof(UpdatorAttribute)] && field != metaDataDictionary[typeof(DateUpdatedAttribute)])
                .Distinct()
                .ToDictionary(field => field, field => new KeyValuePair<string, object>(Clause.GenerateParameterName(), implementationType.GetProperty(field).GetValue(order.DefaultModel)));

            // Identify fields that should not be copied from record to record.
            // Use these fields to produce a list of fields not containing these fields.
            string[] unwantedFields =
                metaDataDictionary
                    .Where(dict => dict.Key != typeof(TableNameAttribute))
                    .Select(dict => dict.Value)
                    .Concat(defaultValues.Select(value => value.Key))
                    .Distinct()
                    .ToArray();
            string[] fieldsToCopy = abstractionType.GetProperties().Where(prop => unwantedFields.Contains(prop.Name) == false).Select(prop => prop.Name).OrderBy(field => field).ToArray();

            // A collection of field names, with accompanying parameters are produced.
            Tuple<string, string>[] fieldParameterNameTupleCollection = defaultValues.Select(value => Tuple.Create(value.Key, value.Value.Key)).ToArray();

            // Here is a callback that can be used to produce the clause that filters the records to be copied.
            Clause tempTableClause = clauseCallback(new RecordCopyClauseCallbackOrder(idSet.Select(id => Tuple.Create(id.Key, id.Value)), fieldParameterNameTupleCollection));

            // Get the record copy query, assemble parameters, and return.
            ISimpleDataOrder transferQuery = Get.RecordCopyQuery(tableName, metaDataDictionary[typeof(IdAttribute)], tempTableClause, fieldParameterNameTupleCollection, fieldsToCopy);

            var parameters = transferQuery
                .Parameters
                .Concat(defaultValues.Select(value => value.Value))
                .Concat(idSet)
                .OrderBy(pair => pair.Key);

            string query =
                wrapInTransaction ?
                    SurroundQueryWithTransaction(transferQuery.Query) :
                    transferQuery.Query;

            return new AliasedCommandTypeDataOrder(query, CommandType.Text, parameters);
        }

    }
}
