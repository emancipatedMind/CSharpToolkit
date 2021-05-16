namespace CSharpToolkit.DataAccess.Abstractions {
    public interface ITransactionScopeProvider {
        ITransactionScopeDataAccessor GetTransactionScopeDataAccessor();
    }
}
