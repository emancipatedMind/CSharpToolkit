namespace CSharpToolkit.DataAccess.Abstractions {
    using CSharpToolkit.Utilities;
    using System.Threading.Tasks;
    public interface IAdminDataAccessor<TModel> {
        Task<OperationResult<int>> CreateAsync(TModel model);
        Task<OperationResult<int>> CreateAsync();
        Task<OperationResult> DeleteAsync(TModel id);
        Task<OperationResult> UpdateAsync(TModel model);
    }
}
