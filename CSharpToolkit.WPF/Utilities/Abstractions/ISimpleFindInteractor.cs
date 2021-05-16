namespace CSharpToolkit.Utilities.Abstractions {
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CSharpToolkit.Utilities;
    using CSharpToolkit.DataAccess.Abstractions;

    public interface ISimpleFindInteractor<TDataCollector, TSearchOrder, TSearchResults> : ISimpleFindDataAccessor<TSearchOrder, TSearchResults> {

        bool AllowSearch(TDataCollector dataCollector);
    }
}
