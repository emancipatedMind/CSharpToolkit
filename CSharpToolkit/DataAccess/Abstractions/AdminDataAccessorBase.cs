namespace CSharpToolkit.DataAccess.Abstractions {
    using CSharpToolkit.DataAccess.Abstractions;
    using CSharpToolkit.Utilities;
    using Extensions;
    using System.Threading.Tasks;
    public abstract class AdminDataAccessorBase<TModel> : IAdminDataAccessor<TModel> {

        IAdminQueryProvider<TModel> _queryProvider;
        IDataAccessor _dataAccessor;

        public AdminDataAccessorBase(IDataAccessor dataAccessor, IAdminQueryProvider<TModel> queryProvider) {
            _dataAccessor = dataAccessor;
            _queryProvider = queryProvider;
        }

        public Task<OperationResult<int>> CreateAsync(TModel model) =>
            Task.Run(() => _dataAccessor.Create(_queryProvider.Create(model)));

        public Task<OperationResult<int>> CreateAsync() =>
            Task.Run(() => _dataAccessor.Create(_queryProvider.Create()));

        public Task<OperationResult> DeleteAsync(TModel model) =>
            Task.Run(() => _dataAccessor.Delete(_queryProvider.Delete(model)));

        public Task<OperationResult> UpdateAsync(TModel model) =>
            Task.Run(() => _dataAccessor.Update(_queryProvider.Update(model)));

    }
}
