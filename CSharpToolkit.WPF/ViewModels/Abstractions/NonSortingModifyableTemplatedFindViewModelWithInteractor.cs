namespace CSharpToolkit.ViewModels.Abstractions {
    using System;
    using System.Collections.Generic;
    using CSharpToolkit.Utilities.Abstractions;
    using CSharpToolkit.Validation.Abstractions;

    public abstract class NonSortingModifyableTemplatedFindViewModelWithInteractor<TDataGridSource, TDataCollector, TSearchOrder, TModifyable> : ModifyableTemplatedFindViewModelWithInteractor<TDataGridSource, TDataCollector, TModifyable, TSearchOrder, TDataGridSource> where TDataGridSource : IIdProvider, TModifyable where TModifyable : IIdProvider {

        public NonSortingModifyableTemplatedFindViewModelWithInteractor(ISimpleFindInteractor<TDataCollector, TSearchOrder, TDataGridSource> interactor, IValidate<string> validator, Func<TDataCollector> dataCollectorInitializer) : base(interactor, validator, dataCollectorInitializer) { }

        protected override IEnumerable<TDataGridSource> DataGridSourceConverter(IEnumerable<TDataGridSource> source) => source;
    }
}
