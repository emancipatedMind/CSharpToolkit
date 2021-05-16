namespace CSharpToolkit.Extensions {
    using DataAccess.Abstractions;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using Utilities;
    using Utilities.Abstractions;
    /// <summary>
    /// Extension methods for the <see cref="IDataAccessor"/>.
    /// </summary>
    public static class DataAccessorExtensions {

        public static OperationResult Update(this IDataAccessor dataAccessor, IAliasedCommandTypeDataOrder dataOrder) =>
            dataAccessor.PerformDataOperationWithDataOrder(dataOrder);

        public static OperationResult Delete(this IDataAccessor dataAccessor, IAliasedCommandTypeDataOrder dataOrder) =>
            dataAccessor.PerformDataOperationWithDataOrder(dataOrder);

        public static OperationResult<int> Create(this IDataAccessor dataAccessor, IAliasedCommandTypeDataOrder dataOrder) {
            OperationResult<object> operation = dataAccessor.SubmitScalarQueryWithDataOrder(dataOrder);
            return
                operation.HadErrors ?
                new OperationResult<int>(operation.Exceptions) :
                    operation.Result is int ?
                        new OperationResult<int>((int)operation.Result) :
                        new OperationResult<int>(0);
        }

        /// <summary>
        /// An extension method for the <see cref="IDataAccessor"/>. Used when only a count is being requested.
        /// </summary>
        /// <param name="dataAccessor">The <see cref="IDataAccessor"/> used to submit query.</param>
        /// <param name="dataOrder">The information for the query.</param>
        /// <returns>A <see cref="Utilities.OperationResult{T}"/> containing count upon success, and Exceptions upon failure.</returns>
        public static OperationResult<int> GetCount(this IDataAccessor dataAccessor, IAliasedCommandTypeDataOrder dataOrder) {
            OperationResult<object> countOperation = dataAccessor.SubmitScalarQueryWithDataOrder(dataOrder);
            return
                countOperation.HadErrors ?
                    new OperationResult<int>(countOperation.Exceptions) :
                    new OperationResult<int>((int)countOperation.Result);
        }

        /// <summary>
        /// An extension method for the <see cref="IDataAccessor"/>. Used to produce a list of objects.
        /// </summary>
        /// <typeparam name="T">The type of object represented by the lookup. The type must have an eligible constructor accepting <see cref="System.Data.DataRow"/> as its first parameter, and <see cref="System.Collections.Generic.IEnumerable{T}"/> of type <see cref="IAlias"/> as its second.</typeparam>
        /// <param name="dataAccessor">The <see cref="IDataAccessor"/> used to submit query.</param>
        /// <param name="dataOrder">The information for the query.</param>
        /// <returns>A <see cref="Utilities.OperationResult{T}"/> containing result upon success, and Exceptions upon failure.</returns>
        public static OperationResult<List<T>> GetTable<T>(this IDataAccessor dataAccessor, IAliasedCommandTypeDataOrder dataOrder) =>
            GetTable<T, T>(dataAccessor, dataOrder);

        /// <summary>
        /// An extension method for the <see cref="IDataAccessor"/>. Used to produce a list of objects when that type is an abstraction using a derived type.
        /// </summary>
        /// <typeparam name="T">The type of object represented by the lookup.</typeparam>
        /// <typeparam name="T2">The type of object represented by the lookup. The type must have an eligible constructor accepting <see cref="System.Data.DataRow"/> as its first parameter, and <see cref="System.Collections.Generic.IEnumerable{T}"/> of type <see cref="IAlias"/> as its second, and must be derived from T.</typeparam>
        /// <param name="dataAccessor">The <see cref="IDataAccessor"/> used to submit query.</param>
        /// <param name="dataOrder">The information for the query.</param>
        /// <returns>A <see cref="Utilities.OperationResult{T}"/> containing result upon success, and Exceptions upon failure.</returns>
        public static OperationResult<List<T>> GetTable<T, T2>(this IDataAccessor dataAccessor, IAliasedCommandTypeDataOrder dataOrder) where T2 : T {
            ConstructorInfo constructor = typeof(T2).GetConstructor(new[] { typeof(DataRow), typeof(IEnumerable<IAlias>) });
            System.Diagnostics.Debug.Assert(constructor != null, "This overload can only be used with a Type that has an eligible constructor, which is one that accepts System.Data.DataRow as its first parameter, and System.Collections.Generic.IEnumerable<CSharpToolkit.Utilities.Abstractions.IAlias> for its second."); 

            var operation = dataAccessor.SubmitQueryWithDataOrder(dataOrder);
            return operation.HadErrors ?
                new OperationResult<List<T>>(operation.Exceptions) :
                new OperationResult<List<T>>(operation.Result.Select(r => (T2)constructor.Invoke(new object[] { r, dataOrder.Aliases })).Cast<T>().ToList());
        }

        /// <summary>
        /// An extension method for the <see cref="IDataAccessor"/>. Used to produce a list of objects.
        /// </summary>
        /// <typeparam name="T">The type of object represented by the lookup.</typeparam>
        /// <param name="dataAccessor">The <see cref="IDataAccessor"/> used to submit query.</param>
        /// <param name="dataOrder">The information for the query.</param>
        /// <param name="converter">A callback used to convert the <see cref="DataRow"/>into an object of type T.</param>
        /// <returns>A <see cref="Utilities.OperationResult{T}"/> containing result upon success, and Exceptions upon failure.</returns>
        public static OperationResult<List<T>> GetTable<T>(this IDataAccessor dataAccessor, IAliasedCommandTypeDataOrder dataOrder, Func<DataRow, T> converter) =>
            dataAccessor.GetTable(dataOrder, (dO, row) => converter(row));

        /// <summary>
        /// An extension method for the <see cref="IDataAccessor"/>. Used to produce a list of objects.
        /// </summary>
        /// <typeparam name="T">The type of object represented by the lookup.</typeparam>
        /// <param name="dataAccessor">The <see cref="IDataAccessor"/> used to submit query.</param>
        /// <param name="dataOrder">The information for the query.</param>
        /// <param name="converter">A callback used to convert the combination of the <see cref="DataRow"/>, and the <see cref="IAliasedCommandTypeDataOrder"/> into an object of type T.</param>
        /// <returns>A <see cref="Utilities.OperationResult{T}"/> containing result upon success, and Exceptions upon failure.</returns>
        public static OperationResult<List<T>> GetTable<T>(this IDataAccessor dataAccessor, IAliasedCommandTypeDataOrder dataOrder, Func<IAliasedCommandTypeDataOrder, DataRow, T> converter) {
            var operation = dataAccessor.SubmitQueryWithDataOrder(dataOrder);
            return operation.HadErrors ?
                new OperationResult<List<T>>(operation.Exceptions) :
                new OperationResult<List<T>>(operation.Result.Select(row => converter(dataOrder, row)).ToList());
        }

        /// <summary>
        /// An extension method for the <see cref="IDataAccessor"/>. Used to perform a lookup with the <see cref="IDataAccessor"/> interface. If the lookup produces no results, the operation is deemed a failure with the <see cref="DataAccess.EmptyResultSetException"/> Exception.
        /// </summary>
        /// <typeparam name="T">The type of object represented by the lookup.</typeparam>
        /// <param name="dataAccessor">The <see cref="IDataAccessor"/> used to submit query.</param>
        /// <param name="dataOrder">The information for the query.</param>
        /// <param name="converter">A callback used to convert the combination of the <see cref="DataRow"/>, and the <see cref="IAliasedCommandTypeDataOrder"/> into an object of type T.</param>
        /// <returns>A <see cref="Utilities.OperationResult{T}"/> containing result upon success, and Exceptions upon failure.</returns>
        public static OperationResult<T> Lookup<T>(this IDataAccessor dataAccessor, IAliasedCommandTypeDataOrder dataOrder, Func<IAliasedCommandTypeDataOrder, DataRow, T> converter) =>
            dataAccessor.Lookup<T, T>(dataOrder, converter);

        /// <summary>
        /// An extension method for the <see cref="IDataAccessor"/>. Used to perform a lookup with the <see cref="IDataAccessor"/> interface. If the lookup produces no results, the operation is deemed a failure with the <see cref="DataAccess.EmptyResultSetException"/> Exception.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <typeparam name="T2">Derived type of T.</typeparam>
        /// <param name="dataAccessor">The <see cref="IDataAccessor"/> used to submit query.</param>
        /// <param name="dataOrder">The information for the query.</param>
        /// <param name="converter">A callback used to convert the combination of the <see cref="DataRow"/>, and the <see cref="IAliasedCommandTypeDataOrder"/> into an object of type T.</param>
        /// <returns>A <see cref="Utilities.OperationResult{T}"/> containing result upon success, and Exceptions upon failure.</returns>
        public static OperationResult<T> Lookup<T, T2>(this IDataAccessor dataAccessor, IAliasedCommandTypeDataOrder dataOrder, Func<IAliasedCommandTypeDataOrder, DataRow, T2> converter) where T2 : T {
            OperationResult<List<DataRow>> operation = dataAccessor.GetTable(dataOrder, r => r);
            return operation.HadErrors ?
                new OperationResult<T>(operation.Exceptions) :
                operation.Result.Any() == false ?
                    new OperationResult<T>(false, default(T)) :
                    new OperationResult<T>(converter(dataOrder, operation.Result[0]));
        }

        /// <summary>
        /// An extension method for the <see cref="IDataAccessor"/>. Used to translate the <see cref="IAliasedCommandTypeDataOrder"/> interface into the <see cref="IDataAccessor.SubmitScalarQuery(string, CommandType, IEnumerable{KeyValuePair{string, object}})"/> method.
        /// </summary>
        /// <param name="dataAccessor">The <see cref="IDataAccessor"/> used to submit query.</param>
        /// <param name="dataOrder">The information for the query.</param>
        /// <returns>A <see cref="Utilities.OperationResult{T}"/> denoting success of operation containing scalar.</returns>
        public static OperationResult<object> SubmitScalarQueryWithDataOrder(this IDataAccessor dataAccessor, IAliasedCommandTypeDataOrder dataOrder) =>
            CUDOperation(dataAccessor.SubmitScalarQuery, dataOrder);

        /// <summary>
        /// An extension method for the <see cref="IDataAccessor"/>. Used to translate the <see cref="IAliasedCommandTypeDataOrder"/> interface into the <see cref="IDataAccessor.PerformDataOperation(string, CommandType, IEnumerable{KeyValuePair{string, object}})"/> method.
        /// </summary>
        /// <param name="dataAccessor">The <see cref="IDataAccessor"/> used to submit query.</param>
        /// <param name="dataOrder">The information for the query.</param>
        /// <returns>A <see cref="Utilities.OperationResult{T}"/> denoting success of operation containing number of affected rows.</returns>
        public static OperationResult<int> PerformDataOperationWithDataOrder(this IDataAccessor dataAccessor, IAliasedCommandTypeDataOrder dataOrder) =>
            CUDOperation(dataAccessor.PerformDataOperation, dataOrder);

        /// <summary>
        /// An extension method for the <see cref="IDataRowProvider"/>. Used to translate the <see cref="IAliasedCommandTypeDataOrder"/> interface into the <see cref="IDataRowProvider.SubmitQuery(string, CommandType, IEnumerable{KeyValuePair{string, object}})"/> method.
        /// </summary>
        /// <param name="dataAccessor">The <see cref="IDataRowProvider"/> used to submit query.</param>
        /// <param name="dataOrder">The information for the query.</param>
        /// <returns>A <see cref="Utilities.OperationResult{T}"/> denoting success of operation containing <see cref="List{T}"/> of <see cref="DataRow"/>.</returns>
        public static OperationResult<List<DataRow>> SubmitQueryWithDataOrder(this IDataRowProvider dataAccessor, IAliasedCommandTypeDataOrder dataOrder) =>
            CUDOperation(dataAccessor.SubmitQuery, dataOrder);

        /// <summary>
        /// An extension method for the <see cref="IDataRowProvider"/>. Used to translate the <see cref="IAliasedCommandTypeDataOrder"/> interface into the <see cref="IDataRowProvider.SubmitQueryForDataSet(string, CommandType, IEnumerable{KeyValuePair{string, object}})"/> method.
        /// </summary>
        /// <param name="dataAccessor">The <see cref="IDataRowProvider"/> used to submit query.</param>
        /// <param name="dataOrder">The information for the query.</param>
        /// <returns>A <see cref="Utilities.OperationResult{T}"/> denoting success of operation containing <see cref="DataSet"/>.</returns>
        public static OperationResult<DataSet> SubmitQueryForDataSetWithDataOrder(this IDataRowProvider dataAccessor, IAliasedCommandTypeDataOrder dataOrder) =>
            CUDOperation(dataAccessor.SubmitQueryForDataSet, dataOrder);

        static OperationResult<T> CUDOperation<T>(Func<string, CommandType, IEnumerable<KeyValuePair<string, object>>, OperationResult<T>> cudOperation, IAliasedCommandTypeDataOrder dataOrder) {
            if (dataOrder.Query.IsMeaningful() == false)
                return new OperationResult<T>(false, default(T));

            OperationResult<T> operation = cudOperation(dataOrder.Query, dataOrder.CommandType, dataOrder.Parameters);
            return operation.HadErrors ?
                new OperationResult<T>(operation.Exceptions) :
                new OperationResult<T>(operation.Result);
        }

    }
}