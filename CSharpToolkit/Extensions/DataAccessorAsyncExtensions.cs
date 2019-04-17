namespace CSharpToolkit.Extensions {
    using DataAccess.Abstractions;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Utilities;
    using Utilities.Abstractions;
    /// <summary>
    /// Asynchronous extension methods for the <see cref="IDataRowProvider"/>, and the <see cref="IDataAccessor"/>.
    /// </summary>
    public static class DataAccessorAsyncExtensions {

        /// <summary>
        /// Asynchronous. An extension method for the <see cref="IDataAccessor"/>. Used when only a count is being requested.
        /// </summary>
        /// <param name="dataRowProvider">The <see cref="IDataAccessor"/> used to submit query.</param>
        /// <param name="dataOrder">The information for the query.</param>
        /// <returns>A <see cref="Task"/> with a result of <see cref="Utilities.OperationResult{T}"/> containing count upon success, and Exceptions upon failure.</returns>
        public static async Task<OperationResult<int>> GetCountAsync(this IDataAccessor dataRowProvider, IAliasedCommandTypeDataOrder dataOrder) {
            OperationResult<object> countOperation = await dataRowProvider.SubmitScalarQueryWithDataOrderAsync(dataOrder);
            var exceptions = new List<Exception>();
            if (countOperation.HadErrors)
                return new OperationResult<int>(countOperation.Exceptions);
            return
                countOperation.Result is int == false ?
                    new OperationResult<int>(new[] { new InvalidCastException($"Cannot convert {countOperation.Result} into integer.") }) :
                    new OperationResult<int>((int)countOperation.Result);
        }

        /// <summary>
        /// Asynchronous. An extension method for the <see cref="IDataRowProvider"/>. Used to produce a list of objects.
        /// </summary>
        /// <typeparam name="T">The type of object represented by the lookup. The type must have an eligible constructor accepting <see cref="System.Data.DataRow"/> as its first parameter, and <see cref="System.Collections.Generic.IEnumerable{T}"/> of type <see cref="IAlias"/> as its second.</typeparam>
        /// <param name="dataRowProvider">The <see cref="IDataRowProvider"/> used to submit query.</param>
        /// <param name="dataOrder">The information for the query.</param>
        /// <returns>A <see cref='Task'/> with a result of <see cref="Utilities.OperationResult{T}"/> containing result upon success, and Exceptions upon failure.</returns>
        public static Task<OperationResult<List<T>>> GetTableAsync<T>(this IDataRowProvider dataRowProvider, IAliasedCommandTypeDataOrder dataOrder) =>
            GetTableAsync<T, T>(dataRowProvider, dataOrder);

        /// <summary>
        /// Asynchronous. An extension method for the <see cref="IDataRowProvider"/>. Used to produce a list of objects when that type is an abstraction using a derived type.
        /// </summary>
        /// <typeparam name="T">The type of object represented by the lookup.</typeparam>
        /// <typeparam name="T2">The type of object represented by the lookup. The type must have an eligible constructor accepting <see cref="System.Data.DataRow"/> as its first parameter, and <see cref="System.Collections.Generic.IEnumerable{T}"/> of type <see cref="IAlias"/> as its second, and must be derived from T.</typeparam>
        /// <param name="dataRowProvider">The <see cref="IDataRowProvider"/> used to submit query.</param>
        /// <param name="dataOrder">The information for the query.</param>
        /// <returns>A <see cref='Task'/> with a result of <see cref="Utilities.OperationResult{T}"/> containing result upon success, and Exceptions upon failure.</returns>
        public static async Task<OperationResult<List<T>>> GetTableAsync<T, T2>(this IDataRowProvider dataRowProvider, IAliasedCommandTypeDataOrder dataOrder) where T2 : T {
            ConstructorInfo constructor = typeof(T2).GetConstructor(new[] { typeof(DataRow), typeof(IEnumerable<IAlias>) });
            System.Diagnostics.Debug.Assert(constructor != null, "This overload can only be used with a Type that has an eligible constructor, which is one that accepts System.Data.DataRow as its first parameter, and System.Collections.Generic.IEnumerable<CSharpToolkit.Utilities.Abstractions.IAlias> for its second."); 

            var operation = await dataRowProvider.SubmitQueryWithDataOrderAsync(dataOrder);
            return operation.HadErrors ?
                new OperationResult<List<T>>(operation.Exceptions) :
                new OperationResult<List<T>>(await Task.Run(() => operation.Result.Select(r => (T2)constructor.Invoke(new object[] { r, dataOrder.Aliases })).Cast<T>().ToList()));
        }

        /// <summary>
        /// Asynchronous. An extension method for the <see cref="IDataRowProvider"/>. Used to produce a list of objects.
        /// </summary>
        /// <typeparam name="T">The type of object represented by the lookup.</typeparam>
        /// <param name="dataRowProvider">The <see cref="IDataRowProvider"/> used to submit query.</param>
        /// <param name="dataOrder">The information for the query.</param>
        /// <param name="converter">A callback used to convert the <see cref="DataRow"/>into an object of type T.</param>
        /// <returns>A <see cref='Task'/> with a result of <see cref="Utilities.OperationResult{T}"/> containing result upon success, and Exceptions upon failure.</returns>
        public static Task<OperationResult<List<T>>> GetTableAsync<T>(this IDataRowProvider dataRowProvider, IAliasedCommandTypeDataOrder dataOrder, Func<DataRow, T> converter) =>
            dataRowProvider.GetTableAsync(dataOrder, (dO, row) => converter(row));

        /// <summary>
        /// Asynchronous. An extension method for the <see cref="IDataRowProvider"/>. Used to produce a list of objects.
        /// </summary>
        /// <typeparam name="T">The type of object represented by the lookup.</typeparam>
        /// <param name="dataRowProvider">The <see cref="IDataRowProvider"/> used to submit query.</param>
        /// <param name="dataOrder">The information for the query.</param>
        /// <param name="converter">A callback used to convert the combination of the <see cref="DataRow"/>, and the <see cref="IAliasedCommandTypeDataOrder"/> into an object of type T.</param>
        /// <returns>A <see cref='Task'/> with a result of <see cref="Utilities.OperationResult{T}"/> containing result upon success, and Exceptions upon failure.</returns>
        public static async Task<OperationResult<List<T>>> GetTableAsync<T>(this IDataRowProvider dataRowProvider, IAliasedCommandTypeDataOrder dataOrder, Func<IAliasedCommandTypeDataOrder, DataRow, T> converter) {
            var operation = await dataRowProvider.SubmitQueryWithDataOrderAsync(dataOrder);
            return operation.HadErrors ?
                new OperationResult<List<T>>(operation.Exceptions) :
                new OperationResult<List<T>>(await Task.Run(() => operation.Result.Select(row => converter(dataOrder, row)).ToList()));
        }

        /// <summary>
        /// An extension method for the <see cref="IDataRowProvider"/>. Used to perform a lookup with the <see cref="IDataRowProvider"/> interface. If the lookup produces no results, the operation is deemed a failure with the <see cref="DataAccess.EmptyResultSetException"/> Exception.
        /// </summary>
        /// <typeparam name="T">The type of object represented by the lookup.</typeparam>
        /// <param name="dataRowProvider">The <see cref="IDataRowProvider"/> used to submit query.</param>
        /// <param name="dataOrder">The information for the query.</param>
        /// <param name="converter">A callback used to convert the combination of the <see cref="DataRow"/>, and the <see cref="IAliasedCommandTypeDataOrder"/> into an object of type T.</param>
        /// <returns>A <see cref='Task'/> with a result of <see cref="Utilities.OperationResult{T}"/> containing result upon success, and Exceptions upon failure.</returns>
        public static Task<OperationResult<T>> LookupAsync<T>(this IDataRowProvider dataRowProvider, IAliasedCommandTypeDataOrder dataOrder, Func<IAliasedCommandTypeDataOrder, DataRow, T> converter) =>
            dataRowProvider.LookupAsync<T, T>(dataOrder, converter);

        /// <summary>
        /// Asynchronous. An extension method for the <see cref="IDataRowProvider"/>. Used to perform a lookup with the <see cref="IDataRowProvider"/> interface. If the lookup produces no results, the operation is deemed a failure with the <see cref="DataAccess.EmptyResultSetException"/> Exception.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <typeparam name="T2">Derived type of T.</typeparam>
        /// <param name="dataRowProvider">The <see cref="IDataRowProvider"/> used to submit query.</param>
        /// <param name="dataOrder">The information for the query.</param>
        /// <param name="converter">A callback used to convert the combination of the <see cref="DataRow"/>, and the <see cref="IAliasedCommandTypeDataOrder"/> into an object of type T.</param>
        /// <returns>A <see cref='Task'/> with a result of <see cref="Utilities.OperationResult{T}"/> containing result upon success, and Exceptions upon failure.</returns>
        public async static Task<OperationResult<T>> LookupAsync<T, T2>(this IDataRowProvider dataRowProvider, IAliasedCommandTypeDataOrder dataOrder, Func<IAliasedCommandTypeDataOrder, DataRow, T2> converter) where T2 : T {
            OperationResult<List<DataRow>> operation = await dataRowProvider.GetTableAsync(dataOrder, r => r);
            return operation.HadErrors ?
                new OperationResult<T>(operation.Exceptions) :
                operation.Result.Any() == false ?
                    new OperationResult<T>(false, default(T)) :
                    new OperationResult<T>(await Task.Run(() => converter(dataOrder, operation.Result[0])));
        }

        /// <summary>
        /// Asynchronous. An extension method for the <see cref="IDataAccessor"/>. Used to translate the <see cref="IAliasedCommandTypeDataOrder"/> interface into the <see cref="IDataAccessor.SubmitScalarQuery(string, CommandType, IEnumerable{KeyValuePair{string, object}})"/> method.
        /// </summary>
        /// <param name="dataAccessor">The <see cref="IDataAccessor"/> used to submit query.</param>
        /// <param name="dataOrder">The information for the query.</param>
        /// <returns>A <see cref='Task'/> with a result of <see cref="Utilities.OperationResult{T}"/> which contains the int returned.</returns>
        public static Task<OperationResult<object>> SubmitScalarQueryWithDataOrderAsync(this IDataAccessor dataAccessor, IAliasedCommandTypeDataOrder dataOrder) =>
            CUDOperationAsync(dataAccessor.SubmitScalarQueryAsync, dataOrder);

        /// <summary>
        /// Asynchronous. An extension method for the <see cref="IDataAccessor"/>. Used to translate the <see cref="IAliasedCommandTypeDataOrder"/> interface into the <see cref="IDataAccessor.PerformDataOperation(string, CommandType, IEnumerable{KeyValuePair{string, object}})"/> method.
        /// </summary>
        /// <param name="dataAccessor">The <see cref="IDataAccessor"/> used to submit query.</param>
        /// <param name="dataOrder">The information for the query.</param>
        /// <returns>A <see cref='Task'/> with a result of <see cref="Utilities.OperationResult{T}"/> denoting success of operation.</returns>
        public static Task<OperationResult<int>> PerformDataOperationWithDataOrderAsync(this IDataAccessor dataAccessor, IAliasedCommandTypeDataOrder dataOrder) =>
            CUDOperationAsync(dataAccessor.PerformDataOperationAsync, dataOrder);

        /// <summary>
        /// Asynchronous. An extension method for the <see cref="IDataRowProvider"/>. Used to translate the <see cref="IAliasedCommandTypeDataOrder"/> interface into the <see cref="IDataRowProvider.SubmitQuery(string, CommandType, IEnumerable{KeyValuePair{string, object}})"/> method.
        /// </summary>
        /// <param name="dataRowProvider">The <see cref="IDataRowProvider"/> used to submit query.</param>
        /// <param name="dataOrder">The information for the query.</param>
        /// <returns>A <see cref='Task'/> with a result of <see cref="Utilities.OperationResult{T}"/> denoting success of operation.</returns>
        public static Task<OperationResult<List<DataRow>>> SubmitQueryWithDataOrderAsync(this IDataRowProvider dataRowProvider, IAliasedCommandTypeDataOrder dataOrder) =>
            CUDOperationAsync(dataRowProvider.SubmitQueryAsync, dataOrder);

        static async Task<OperationResult<T>> CUDOperationAsync<T>(Func<string, CommandType, IEnumerable<KeyValuePair<string, object>>, Task<OperationResult<T>>> cudOperation, IAliasedCommandTypeDataOrder dataOrder) {
            if (dataOrder.Query.IsMeaningful() == false)
                return new OperationResult<T>(false, default(T));

            OperationResult<T> operation = await cudOperation(dataOrder.Query, dataOrder.CommandType, dataOrder.Parameters);
            return operation.HadErrors ?
                new OperationResult<T>(operation.Exceptions) :
                new OperationResult<T>(operation.Result);
        }

        /****************************************************************************

            BEGIN -- Faulted Recording Method.

        ****************************************************************************/

        /// <summary>
        /// Asynchronous. An extension method for the <see cref="IDataAccessor"/>. Used when only a count is being requested. If the operation fails, the incoming Type, and method name will added to Exception.
        /// </summary>
        /// <param name="dataRowProvider">The <see cref="IDataAccessor"/> used to submit query.</param>
        /// <param name="dataOrder">The information for the query.</param>
        /// <param name="declaringTypeCallback">The callback to define the type to be added to the <see cref="Utilities.OperationResult{T}"/> if faulted.</param>
        /// <param name="callingMethod">The callback to define the type to be added to the <see cref="Utilities.OperationResult{T}"/> if faulted.</param>
        /// <returns>A <see cref="Task"/> with a result of <see cref="Utilities.OperationResult{T}"/> containing count upon success, and Exceptions upon failure.</returns>
        public static async Task<OperationResult<int>> GetCountAsync(this IDataAccessor dataRowProvider, IAliasedCommandTypeDataOrder dataOrder, Func<Type> declaringTypeCallback, string callingMethod) {
            OperationResult<object> operation = await dataRowProvider.SubmitScalarQueryWithDataOrderAsync(dataOrder, declaringTypeCallback, callingMethod);
            return
                operation.HadErrors ?
                    new OperationResult<int>(operation.Exceptions) :
                    new OperationResult<int>((int)operation.Result);
        }

        /// <summary>
        /// Asynchronous. An extension method for the <see cref="IDataRowProvider"/>. Used to produce a list of objects. If the operation fails, the incoming Type, and method name will added to Exception.
        /// </summary>
        /// <typeparam name="T">The type of object represented by the lookup. The type must have an eligible constructor accepting <see cref="System.Data.DataRow"/> as its first parameter, and <see cref="System.Collections.Generic.IEnumerable{T}"/> of type <see cref="IAlias"/> as its second.</typeparam>
        /// <param name="dataRowProvider">The <see cref="IDataRowProvider"/> used to submit query.</param>
        /// <param name="dataOrder">The information for the query.</param>
        /// <param name="declaringTypeCallback">The callback to define the type to be added to the <see cref="Utilities.OperationResult{T}"/> if faulted.</param>
        /// <param name="callingMethod">The callback to define the type to be added to the <see cref="Utilities.OperationResult{T}"/> if faulted.</param>
        /// <returns>A <see cref='Task'/> with a result of <see cref="Utilities.OperationResult{T}"/> containing result upon success, and Exceptions upon failure.</returns>
        public static Task<OperationResult<List<T>>> GetTableAsync<T>(this IDataRowProvider dataRowProvider, IAliasedCommandTypeDataOrder dataOrder, Func<Type> declaringTypeCallback, string callingMethod) =>
            GetTableAsync<T, T>(dataRowProvider, dataOrder, declaringTypeCallback, callingMethod);

        /// <summary>
        /// Asynchronous. An extension method for the <see cref="IDataRowProvider"/>. Used to produce a list of objects when that type is an abstraction using a derived type. If the operation fails, the incoming Type, and method name will added to Exception.
        /// </summary>
        /// <typeparam name="T">The type of object represented by the lookup.</typeparam>
        /// <typeparam name="T2">The type of object represented by the lookup. The type must have an eligible constructor accepting <see cref="System.Data.DataRow"/> as its first parameter, and <see cref="System.Collections.Generic.IEnumerable{T}"/> of type <see cref="IAlias"/> as its second, and must be derived from T.</typeparam>
        /// <param name="dataRowProvider">The <see cref="IDataRowProvider"/> used to submit query.</param>
        /// <param name="dataOrder">The information for the query.</param>
        /// <param name="declaringTypeCallback">The callback to define the type to be added to the <see cref="Utilities.OperationResult{T}"/> if faulted.</param>
        /// <param name="callingMethod">The callback to define the type to be added to the <see cref="Utilities.OperationResult{T}"/> if faulted.</param>
        /// <returns>A <see cref='Task'/> with a result of <see cref="Utilities.OperationResult{T}"/> containing result upon success, and Exceptions upon failure.</returns>
        public static async Task<OperationResult<List<T>>> GetTableAsync<T, T2>(this IDataRowProvider dataRowProvider, IAliasedCommandTypeDataOrder dataOrder, Func<Type> declaringTypeCallback, string callingMethod) where T2 : T {
            ConstructorInfo constructor = typeof(T2).GetConstructor(new[] { typeof(DataRow), typeof(IEnumerable<IAlias>) });
            System.Diagnostics.Debug.Assert(constructor != null, "This overload can only be used with a Type that has an eligible constructor, which is one that accepts System.Data.DataRow as its first parameter, and System.Collections.Generic.IEnumerable<CSharpToolkit.Utilities.Abstractions.IAlias> for its second."); 

            var operation = await dataRowProvider.SubmitQueryWithDataOrderAsync(dataOrder, declaringTypeCallback, callingMethod);
            return operation.HadErrors ?
                new OperationResult<List<T>>(operation.Exceptions) :
                new OperationResult<List<T>>(await Task.Run(() => operation.Result.Select(r => (T2)constructor.Invoke(new object[] { r, dataOrder.Aliases })).Cast<T>().ToList()));
        }

        /// <summary>
        /// Asynchronous. An extension method for the <see cref="IDataRowProvider"/>. Used to produce a list of objects. If the operation fails, the incoming Type, and method name will added to Exception.
        /// </summary>
        /// <typeparam name="T">The type of object represented by the lookup.</typeparam>
        /// <param name="dataRowProvider">The <see cref="IDataRowProvider"/> used to submit query.</param>
        /// <param name="dataOrder">The information for the query.</param>
        /// <param name="converter">A callback used to convert the <see cref="DataRow"/>into an object of type T.</param>
        /// <param name="declaringTypeCallback">The callback to define the type to be added to the <see cref="Utilities.OperationResult{T}"/> if faulted.</param>
        /// <param name="callingMethod">The callback to define the type to be added to the <see cref="Utilities.OperationResult{T}"/> if faulted.</param>
        /// <returns>A <see cref='Task'/> with a result of <see cref="Utilities.OperationResult{T}"/> containing result upon success, and Exceptions upon failure.</returns>
        public static Task<OperationResult<List<T>>> GetTableAsync<T>(this IDataRowProvider dataRowProvider, IAliasedCommandTypeDataOrder dataOrder, Func<DataRow, T> converter, Func<Type> declaringTypeCallback, string callingMethod) =>
            dataRowProvider.GetTableAsync(dataOrder, (dO, row) => converter(row), declaringTypeCallback, callingMethod);

        /// <summary>
        /// Asynchronous. An extension method for the <see cref="IDataRowProvider"/>. Used to produce a list of objects. If the operation fails, the incoming Type, and method name will added to Exception.
        /// </summary>
        /// <typeparam name="T">The type of object represented by the lookup.</typeparam>
        /// <param name="dataRowProvider">The <see cref="IDataRowProvider"/> used to submit query.</param>
        /// <param name="dataOrder">The information for the query.</param>
        /// <param name="converter">A callback used to convert the combination of the <see cref="DataRow"/>, and the <see cref="IAliasedCommandTypeDataOrder"/> into an object of type T.</param>
        /// <param name="declaringTypeCallback">The callback to define the type to be added to the <see cref="Utilities.OperationResult{T}"/> if faulted.</param>
        /// <param name="callingMethod">The callback to define the type to be added to the <see cref="Utilities.OperationResult{T}"/> if faulted.</param>
        /// <returns>A <see cref='Task'/> with a result of <see cref="Utilities.OperationResult{T}"/> containing result upon success, and Exceptions upon failure.</returns>
        public static async Task<OperationResult<List<T>>> GetTableAsync<T>(this IDataRowProvider dataRowProvider, IAliasedCommandTypeDataOrder dataOrder, Func<IAliasedCommandTypeDataOrder, DataRow, T> converter, Func<Type> declaringTypeCallback, string callingMethod) {
            var operation = await dataRowProvider.SubmitQueryWithDataOrderAsync(dataOrder, declaringTypeCallback, callingMethod);
            return
                operation.HadErrors ?
                new OperationResult<List<T>>(operation.Exceptions) :
                new OperationResult<List<T>>(await Task.Run(() => operation.Result.Select(row => converter(dataOrder, row)).ToList()));
        }

        /// <summary>
        /// An extension method for the <see cref="IDataRowProvider"/>. Used to perform a lookup with the <see cref="IDataRowProvider"/> interface. If the lookup produces no results, the operation is deemed a failure with the <see cref="DataAccess.EmptyResultSetException"/> Exception. If the operation fails, the incoming Type, and method name will added to Exception.
        /// </summary>
        /// <typeparam name="T">The type of object represented by the lookup.</typeparam>
        /// <param name="dataRowProvider">The <see cref="IDataRowProvider"/> used to submit query.</param>
        /// <param name="dataOrder">The information for the query.</param>
        /// <param name="converter">A callback used to convert the combination of the <see cref="DataRow"/>, and the <see cref="IAliasedCommandTypeDataOrder"/> into an object of type T.</param>
        /// <param name="declaringTypeCallback">The callback to define the type to be added to the <see cref="Utilities.OperationResult{T}"/> if faulted.</param>
        /// <param name="callingMethod">The callback to define the type to be added to the <see cref="Utilities.OperationResult{T}"/> if faulted.</param>
        /// <returns>A <see cref='Task'/> with a result of <see cref="Utilities.OperationResult{T}"/> containing result upon success, and Exceptions upon failure.</returns>
        public static Task<OperationResult<T>> LookupAsync<T>(this IDataRowProvider dataRowProvider, IAliasedCommandTypeDataOrder dataOrder, Func<IAliasedCommandTypeDataOrder, DataRow, T> converter, Func<Type> declaringTypeCallback, string callingMethod) =>
            dataRowProvider.LookupAsync<T, T>(dataOrder, converter, declaringTypeCallback, callingMethod);

        /// <summary>
        /// Asynchronous. An extension method for the <see cref="IDataRowProvider"/>. Used to perform a lookup with the <see cref="IDataRowProvider"/> interface. If the lookup produces no results, the operation is deemed a failure with the <see cref="DataAccess.EmptyResultSetException"/> Exception. If the operation fails, the incoming Type, and method name will added to Exception.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <typeparam name="T2">Derived type of T.</typeparam>
        /// <param name="dataRowProvider">The <see cref="IDataRowProvider"/> used to submit query.</param>
        /// <param name="dataOrder">The information for the query.</param>
        /// <param name="converter">A callback used to convert the combination of the <see cref="DataRow"/>, and the <see cref="IAliasedCommandTypeDataOrder"/> into an object of type T.</param>
        /// <param name="declaringTypeCallback">The callback to define the type to be added to the <see cref="Utilities.OperationResult{T}"/> if faulted.</param>
        /// <param name="callingMethod">The callback to define the type to be added to the <see cref="Utilities.OperationResult{T}"/> if faulted.</param>
        /// <returns>A <see cref='Task'/> with a result of <see cref="Utilities.OperationResult{T}"/> containing result upon success, and Exceptions upon failure.</returns>
        public async static Task<OperationResult<T>> LookupAsync<T, T2>(this IDataRowProvider dataRowProvider, IAliasedCommandTypeDataOrder dataOrder, Func<IAliasedCommandTypeDataOrder, DataRow, T2> converter, Func<Type> declaringTypeCallback, string callingMethod) where T2 : T {
            OperationResult<List<DataRow>> operation = await dataRowProvider.GetTableAsync(dataOrder, r => r, declaringTypeCallback, callingMethod);
            return operation.HadErrors ?
                new OperationResult<T>(operation.Exceptions) :
                operation.Result.Any() == false ?
                    new OperationResult<T>(false, default(T)) :
                    new OperationResult<T>(await Task.Run(() => converter(dataOrder, operation.Result[0])));
        }

        /// <summary>
        /// Asynchronous. An extension method for the <see cref="IDataAccessor"/>. Used to translate the <see cref="IAliasedCommandTypeDataOrder"/> interface into the <see cref="IDataAccessor.SubmitScalarQuery(string, CommandType, IEnumerable{KeyValuePair{string, object}})"/> method. If the operation fails, the incoming Type, and method name will added to Exception.
        /// </summary>
        /// <param name="dataAccessor">The <see cref="IDataAccessor"/> used to submit query.</param>
        /// <param name="dataOrder">The information for the query.</param>
        /// <param name="declaringTypeCallback">The callback to define the type to be added to the <see cref="Utilities.OperationResult{T}"/> if faulted.</param>
        /// <param name="callingMethod">The callback to define the type to be added to the <see cref="Utilities.OperationResult{T}"/> if faulted.</param>
        /// <returns>A <see cref='Task'/> with a result of <see cref="Utilities.OperationResult{T}"/> which contains scalar.</returns>
        public static Task<OperationResult<object>> SubmitScalarQueryWithDataOrderAsync(this IDataAccessor dataAccessor, IAliasedCommandTypeDataOrder dataOrder, Func<Type> declaringTypeCallback, string callingMethod) =>
            CUDOperationAsync(dataAccessor.SubmitScalarQueryAsync, dataOrder, declaringTypeCallback, callingMethod);

        /// <summary>
        /// Asynchronous. An extension method for the <see cref="IDataAccessor"/>. Used to translate the <see cref="IAliasedCommandTypeDataOrder"/> interface into the <see cref="IDataAccessor.PerformDataOperation(string, CommandType, IEnumerable{KeyValuePair{string, object}})"/> method. If the operation fails, the incoming Type, and method name will added to Exception.
        /// </summary>
        /// <param name="dataAccessor">The <see cref="IDataAccessor"/> used to submit query.</param>
        /// <param name="dataOrder">The information for the query.</param>
        /// <param name="declaringTypeCallback">The callback to define the type to be added to the <see cref="Utilities.OperationResult{T}"/> if faulted.</param>
        /// <param name="callingMethod">The callback to define the type to be added to the <see cref="Utilities.OperationResult{T}"/> if faulted.</param>
        /// <returns>A <see cref='Task'/> with a result of <see cref="Utilities.OperationResult{T}"/> denoting success of operation containing number of affected rows.</returns>
        public static Task<OperationResult<int>> PerformDataOperationWithDataOrderAsync(this IDataAccessor dataAccessor, IAliasedCommandTypeDataOrder dataOrder, Func<Type> declaringTypeCallback, string callingMethod) =>
            CUDOperationAsync(dataAccessor.PerformDataOperationAsync, dataOrder, declaringTypeCallback, callingMethod);

        /// <summary>
        /// Asynchronous. An extension method for the <see cref="IDataRowProvider"/>. Used to translate the <see cref="IAliasedCommandTypeDataOrder"/> interface into the <see cref="IDataRowProvider.SubmitQuery(string, CommandType, IEnumerable{KeyValuePair{string, object}})"/> method. If the operation fails, the incoming Type, and method name will added to Exception.
        /// </summary>
        /// <param name="dataRowProvider">The <see cref="IDataRowProvider"/> used to submit query.</param>
        /// <param name="dataOrder">The information for the query.</param>
        /// <param name="declaringTypeCallback">The callback to define the type to be added to the <see cref="Utilities.OperationResult{T}"/> if faulted.</param>
        /// <param name="callingMethod">The callback to define the type to be added to the <see cref="Utilities.OperationResult{T}"/> if faulted.</param>
        /// <returns>A <see cref='Task'/> with a result of <see cref="Utilities.OperationResult{T}"/> denoting success of operation  containing a <see cref="List{T}"/> of <see cref="DataRow"/>.</returns>
        public static Task<OperationResult<List<DataRow>>> SubmitQueryWithDataOrderAsync(this IDataRowProvider dataRowProvider, IAliasedCommandTypeDataOrder dataOrder, Func<Type> declaringTypeCallback, string callingMethod) =>
            CUDOperationAsync(dataRowProvider.SubmitQueryAsync, dataOrder, declaringTypeCallback, callingMethod);

        /// <summary>
        /// Asynchronous. An extension method for the <see cref="IDataRowProvider"/>. Used to translate the <see cref="IAliasedCommandTypeDataOrder"/> interface into the <see cref="IDataRowProvider.SubmitQueryForDataSet(string, CommandType, IEnumerable{KeyValuePair{string, object}})"/> method. If the operation fails, the incoming Type, and method name will added to Exception.
        /// </summary>
        /// <param name="dataRowProvider">The <see cref="IDataRowProvider"/> used to submit query.</param>
        /// <param name="dataOrder">The information for the query.</param>
        /// <param name="declaringTypeCallback">The callback to define the type to be added to the <see cref="Utilities.OperationResult{T}"/> if faulted.</param>
        /// <param name="callingMethod">The callback to define the type to be added to the <see cref="Utilities.OperationResult{T}"/> if faulted.</param>
        /// <returns>A <see cref='Task'/> with a result of <see cref="Utilities.OperationResult{T}"/> denoting success of operation containing a <see cref="DataSet"/>.</returns>
        public static Task<OperationResult<DataSet>> SubmitQueryForDataSetWithDataOrderAsync(this IDataRowProvider dataRowProvider, IAliasedCommandTypeDataOrder dataOrder, Func<Type> declaringTypeCallback, string callingMethod) =>
            CUDOperationAsync(dataRowProvider.SubmitQueryForDataSetAsync, dataOrder, declaringTypeCallback, callingMethod);

        static async Task<OperationResult<T>> CUDOperationAsync<T>(Func<string, CommandType, IEnumerable<KeyValuePair<string, object>>, Task<OperationResult<T>>> cudOperation, IAliasedCommandTypeDataOrder dataOrder, Func<Type> declaringTypeCallback, string callingMethod) {
            if (dataOrder.Query.IsMeaningful() == false)
                return new OperationResult<T>(false, default(T));

            OperationResult<T> operation = await cudOperation(dataOrder.Query, dataOrder.CommandType, dataOrder.Parameters);
            if (operation.HadErrors)
                Perform.ExceptionTypeAndMethodRecord(operation, declaringTypeCallback(), callingMethod);
            return
                operation.HadErrors ?
                new OperationResult<T>(operation.Exceptions) :
                new OperationResult<T>(operation.Result);
        }

        /****************************************************************************

            END -- Faulted Recording Method.

        ****************************************************************************/

        /// <summary>
        /// Asynchronous.  Performs submission of query to return a single value.
        /// </summary>
        /// <param name="dataAccessor">The <see cref="IDataAccessor"/> used to submit query.</param>
        /// <param name="sql">Query text.</param>
        /// <param name="commandType">Type of command.</param>
        /// <param name="parameters">Parameters for query.</param>
        /// <returns>A <see cref="Task"/> with a result of an <see cref="OperationResult"/> containing the value of the first field found by query.</returns>
        public static Task<OperationResult<object>> SubmitScalarQueryAsync(this IDataAccessor dataAccessor, string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters) =>
            Task.Run(() => dataAccessor.SubmitScalarQuery(sql, commandType, parameters));

        /// <summary>
        /// Asynchronous. Performs a data operation that returns number of rows affected.
        /// </summary>
        /// <param name="dataAccessor">The <see cref="IDataAccessor"/> used to submit query.</param>
        /// <param name="sql">Query text.</param>
        /// <param name="commandType">Type of command.</param>
        /// <param name="parameters">Parameters for query.</param>
        /// <returns>A <see cref="Task"/> with a result of an <see cref="OperationResult"/> containing number of rows affected.</returns>
        public static Task<OperationResult<int>> PerformDataOperationAsync(this IDataAccessor dataAccessor, string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters) =>
            Task.Run(() => dataAccessor.PerformDataOperation(sql, commandType, parameters));

        /// <summary>
        /// Asynchronous. Performs submission of query.
        /// </summary>
        /// <param name="dataRowProvider">The <see cref="IDataRowProvider"/> used to submit query.</param>
        /// <param name="sql">Query text.</param>
        /// <param name="commandType">Type of command.</param>
        /// <param name="parameters">Parameters for query.</param>
        /// <returns>A <see cref="Task"/> with a result of an <see cref="OperationResult"/> containing a <see cref="List{T}"/> of <see cref="DataRow"/>.</returns>
        public static Task<OperationResult<List<DataRow>>> SubmitQueryAsync(this IDataRowProvider dataRowProvider, string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters) =>
            Task.Run(() => dataRowProvider.SubmitQuery(sql, commandType, parameters));

        /// <summary>
        /// Asynchronous. Performs submission of query to receive datasets.
        /// </summary>
        /// <param name="dataRowProvider">The <see cref="IDataRowProvider"/> used to submit query.</param>
        /// <param name="sql">Query text.</param>
        /// <param name="commandType">Type of command.</param>
        /// <param name="parameters">Parameters for query.</param>
        /// <returns>A <see cref="Task"/> with a result of an <see cref="OperationResult"/> containing a <see cref="DataSet"/>.</returns>
        public static Task<OperationResult<DataSet>> SubmitQueryForDataSetAsync(this IDataRowProvider dataRowProvider, string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters) =>
            Task.Run(() => dataRowProvider.SubmitQueryForDataSet(sql, commandType, parameters));

    }
}