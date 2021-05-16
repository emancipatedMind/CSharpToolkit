namespace CSharpToolkit.ViewModels.Abstractions {
    using System;
    using System.Collections.Generic;
    using CSharpToolkit.Utilities.Abstractions;
    using CSharpToolkit.Validation.Abstractions;
    using CSharpToolkit.DataAccess.Abstractions;

    public abstract class NonSortingModifyableTemplatedFindViewModel<TDataGridSource, TDataCollector, TSearchOrder, TModifyable> : ModifyableTemplatedFindViewModel<TDataGridSource, TDataCollector, TModifyable, TSearchOrder, TDataGridSource> where TDataGridSource : IIdProvider, TModifyable where TModifyable : IIdProvider {

        public NonSortingModifyableTemplatedFindViewModel(ISimpleFindDataAccessor<TSearchOrder, TDataGridSource> dataAccessor, IValidate<string> validator, Func<TDataCollector> dataCollectorInitializer) : base(dataAccessor, validator, dataCollectorInitializer) { }

        protected override IEnumerable<TDataGridSource> DataGridSourceConverter(IEnumerable<TDataGridSource> source) => source;
    }
}
