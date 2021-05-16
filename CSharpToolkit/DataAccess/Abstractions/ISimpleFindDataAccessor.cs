namespace CSharpToolkit.DataAccess.Abstractions {
    using System.Collections.Generic;
    using CSharpToolkit.Utilities;
    using System.Threading.Tasks;
    public interface ISimpleFindDataAccessor<TOrder, TDataGridSource> {
        Task<OperationResult<List<TDataGridSource>>> SubmitSearchOrderAsync(TOrder dataCollector);
    }
}
