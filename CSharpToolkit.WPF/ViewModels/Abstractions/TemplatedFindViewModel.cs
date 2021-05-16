namespace CSharpToolkit.ViewModels.Abstractions {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CSharpToolkit.Utilities.Abstractions;
    using CSharpToolkit.Validation.Abstractions;
    using CSharpToolkit.Utilities;
    using CSharpToolkit.DataAccess.Abstractions;

    [Obsolete("Use the ModifyableTemplatedFindViewModel instead.")]
    public abstract class TemplatedFindViewModel<TDataGridSource, TDataCollector, TSearchOrder, TSearchReturn> : FindViewModel<TDataGridSource, TDataCollector> where TDataGridSource : IIdProvider {

        ISimpleFindDataAccessor<TSearchOrder, TSearchReturn> _dataAccessor;

        public TemplatedFindViewModel(ISimpleFindDataAccessor<TSearchOrder, TSearchReturn> dataAccessor, IValidate<string> validator, Func<TDataCollector> dataCollectorInitializer) : base(validator, dataCollectorInitializer) {
            _dataAccessor = dataAccessor;
        }

        protected override Task SearchCallback() =>
            LockFormForExecution(async () => {
                OnSearchToBegin();

                ItemsFound.Clear();
                await ItemsFoundClearedBeforeSearch();

                OperationResult<List<TSearchReturn>> operation =
                    await _dataAccessor.SubmitSearchOrderAsync(DataCollectorConverter(DataCollector));

                if (operation.WasSuccessful)
                    await Perform.CollectionRehydrationAsync(ItemsFound, DataGridSourceConverter(operation.Result));
                await ItemsFoundFillComplete();
                if (ItemsFound.Count == 1)
                    SelectedItem = ItemsFound.First();

                OnSearchComplete(operation);
            });

        public override Task RefreshCollections() => Task.CompletedTask;

        protected virtual Task ItemsFoundFillComplete() => Task.CompletedTask;
        protected virtual Task ItemsFoundClearedBeforeSearch() => Task.CompletedTask;

        protected abstract TSearchOrder DataCollectorConverter(TDataCollector dataCollector);

        protected abstract IEnumerable<TDataGridSource> DataGridSourceConverter(IEnumerable<TSearchReturn> source);
    }
}
