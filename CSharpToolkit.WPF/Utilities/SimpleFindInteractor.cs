namespace DAILibrary.Resources {
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Abstractions;
    using CSharpToolkit.Utilities;
    using Extensions;
    using CSharpToolkit.DataAccess.Abstractions;
    using CSharpToolkit.Extensions;
    using CSharpToolkit.Utilities.Abstractions;

    public class SimpleFindInteractor<TDataCollector, TOrder, TDataGridSource> : ISimpleFindInteractor<TDataCollector, TOrder, TDataGridSource> {

        ISimpleFindDataAccessor<TOrder, TDataGridSource> _component;
        IApplication _application;

        public SimpleFindInteractor(ISimpleFindDataAccessor<TOrder, TDataGridSource> component, IApplication application) {
            _component = component;
            _application = application;
        }

        public virtual Task<OperationResult<List<TDataGridSource>>> SubmitSearchOrderAsync(TOrder dataCollector) =>
            _application.ErrorLogger.ErrorLogIfOperationFaultedAsync(GetType, nameof(SubmitSearchOrderAsync), () => _component.SubmitSearchOrderAsync(dataCollector));

        public virtual bool AllowSearch(TDataCollector dataCollector) => true;
    }
}