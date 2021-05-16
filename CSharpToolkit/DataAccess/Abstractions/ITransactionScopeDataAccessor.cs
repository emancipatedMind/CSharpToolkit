namespace CSharpToolkit.DataAccess.Abstractions {
    using CSharpToolkit.Utilities;
    public interface ITransactionScopeDataAccessor : IDataAccessor {
        OperationResult CommitTransaction();
        OperationResult RollbackTransaction();
    }
}
