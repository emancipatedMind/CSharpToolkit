namespace CSharpToolkit.Extensions {
    using DataAccess.Abstractions;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Utilities;

    public static class TransactionDataAccessorAsyncExtensions {

        public static async Task<OperationResult<T>> RunWithTransactionAsync<T>(this ITransactionScopeProvider provider, Func<ITransactionScopeDataAccessor, Task<OperationResult<T>>> callback) {
            ITransactionScopeDataAccessor accessor = null;
            try {

                accessor = provider.GetTransactionScopeDataAccessor();

                var operation = await callback(accessor);

                if (operation.WasSuccessful) {
                    var commitTransaction = accessor.CommitTransaction();
                    if (commitTransaction.WasSuccessful == false)
                        return new OperationResult<T>(commitTransaction.Exceptions);
                }
                else {
                    var rollbackTransaction = accessor.RollbackTransaction();
                    if (rollbackTransaction.WasSuccessful == false)
                        return new OperationResult<T>(false, operation.Result, operation.Exceptions.Concat(rollbackTransaction.Exceptions));
                }

                return operation;
            }
            catch(Exception ex) {
                var rollBack =
                    accessor?.RollbackTransaction();

                var exceptions = new List<Exception>();
                exceptions.Add(ex);

                if (rollBack != null)
                    exceptions.AddRange(rollBack.Exceptions);

                return new OperationResult<T>(exceptions);
            } 
        } 

        public static async Task<OperationResult> RunWithTransactionAsync(this ITransactionScopeProvider provider, Func<ITransactionScopeDataAccessor, Task<OperationResult>> callback) {
            ITransactionScopeDataAccessor accessor = null;
            try {
                accessor = provider.GetTransactionScopeDataAccessor();

                var operation = await callback(accessor);

                if (operation.WasSuccessful) {
                    var commitTransaction = accessor.CommitTransaction();
                    if (commitTransaction.WasSuccessful == false)
                        return new OperationResult(commitTransaction.Exceptions);
                }
                else {
                    var rollbackTransaction = accessor.RollbackTransaction();
                    if (rollbackTransaction.WasSuccessful == false)
                        return new OperationResult(operation.Exceptions.Concat(rollbackTransaction.Exceptions));
                }

                return operation;
            }
            catch(Exception ex) {
                var rollBack =
                accessor?.RollbackTransaction();

                var exceptions = new List<Exception>();
                exceptions.Add(ex);

                if (rollBack != null)
                    exceptions.AddRange(rollBack.Exceptions);

                return new OperationResult(exceptions);
            } 
        } 

    }
}
