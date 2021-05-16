namespace CSharpToolkit.DataAccess.Abstractions {
    using CSharpToolkit.Utilities;
    using System.Threading.Tasks;
    public interface ISimpleAdminDataAccessor<TDataOrder, TReturnType> : IAdminDataAccessor<TDataOrder> {
        Task<OperationResult<TReturnType>> LookupAsync(TDataOrder model);
    }
    public interface ISimpleAdminDataAccessor<T> : ISimpleAdminDataAccessor<T, T> { }
}
