namespace CSharpToolkit.ViewModels.Abstractions {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CSharpToolkit.Utilities;
    using CSharpToolkit.Utilities.Abstractions;
    using CSharpToolkit.Validation.Abstractions;
    using CSharpToolkit.DataAccess.Abstractions;

    /// <summary>
    /// Extends the <see cref="ModifyableFindViewModel{TDataGridSource, TDataCollector, TModifyable}"/> class, and implements the <see cref="SearchCallback"/> using an object which adorns the <see cref="ISimpleFindDataAccessor{TSearchOrder, TSearchReturn}"/> interface.
    /// </summary>
    /// <typeparam name="TDataGridSource">The type that represents what is being presented by <see cref="FindViewModel{TDataGridSource, TDataCollector}.ItemsFound"/>, and <see cref="FindViewModel{TDataGridSource, TDataCollector}.SelectedItem"/>. Must implement <see cref="IIdProvider"/>, and <typeparamref name="TModifyable"/>.</typeparam>
    /// <typeparam name="TDataCollector">The type that represents what will hold the data being gathered.</typeparam>
    /// <typeparam name="TModifyable">The type that represents what will be modifyable. Must implement <see cref="IIdProvider"/>.</typeparam>
    /// <typeparam name="TSearchOrder">The type that represents what is used by the DataAccessor for transmission of search criteria.</typeparam>
    /// <typeparam name="TSearchReturn">The type that represents what is used by the DataAccessor for transmission of search results.</typeparam>
    public abstract class ModifyableTemplatedFindViewModel<TDataGridSource, TDataCollector, TModifyable, TSearchOrder, TSearchReturn> : ModifyableFindViewModel<TDataGridSource, TDataCollector, TModifyable> where TDataGridSource : TModifyable where TModifyable : IIdProvider {
        ISimpleFindDataAccessor<TSearchOrder, TSearchReturn> _dataAccessor;
        bool _disposed = false;
        bool _attemptDisposalAfterClear;

        /// <summary>
        /// Instantiates the <see cref="ModifyableTemplatedFindViewModel{TDataGridSource, TDataCollector, TModifyable, TSearchOrder, TSearchReturn}"/> class.
        /// </summary>
        /// <param name="dataAccessor">The object which fills the role of data access.</param>
        /// <param name="validator">The validator used.</param>
        /// <param name="dataCollectorInitializer">A function which initializes the <see cref="DataCollector"/>.</param>
        public ModifyableTemplatedFindViewModel(ISimpleFindDataAccessor<TSearchOrder, TSearchReturn> dataAccessor, IValidate<string> validator, Func<TDataCollector> dataCollectorInitializer) : base(validator, dataCollectorInitializer) {
            _dataAccessor = dataAccessor;
        }

        public override Task RefreshCollections() => Task.CompletedTask;

        /// <summary>
        /// If true, attaches event to <see cref="ItemsFound"/> that will call <see cref="System.IDisposable.Dispose"/> if implemented on each item is removed.
        /// </summary>
        public bool AttemptDisposalAfterClear {
            get { return _attemptDisposalAfterClear; }
            set {
                if (Perform.ReplaceIfDifferent(ref _attemptDisposalAfterClear, value).WasSuccessful) {
                    if (_attemptDisposalAfterClear) {
                        ItemsFound.CollectionChanged += ItemsFound_CollectionChanged;
                    }
                    else {
                        ItemsFound.CollectionChanged -= ItemsFound_CollectionChanged;
                    }
                }
            }
        }

        public bool SelectIfOnlyOneItemReturned { get; set; } = true;

        private void ItemsFound_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            if ((e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset || e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove) && e.OldItems != null) {
                foreach (var item in e.OldItems) {
                    (item as IDisposable)?.Dispose();
                }
            }
        }

        protected override Task SearchCallback() =>
            LockFormForExecution(async () => {
                OnSearchToBegin();

                var itemList = new List<TDataGridSource>();
                await ItemsFoundToBeCleared(ItemsFound);

                if (AttemptDisposalAfterClear) {
                    itemList.AddRange(ItemsFound ?? Enumerable.Empty<TDataGridSource>());
                }

                ItemsFound.ToList().ForEach(item => ItemsFound.Remove(item));
                await ItemsFoundClearedBeforeSearch();

                OperationResult<List<TSearchReturn>> operation =
                    await _dataAccessor.SubmitSearchOrderAsync(DataCollectorConverter(DataCollector));

                if (_disposed)
                    return;

                if (operation.WasSuccessful) {
                    await Perform.CollectionRehydrationAsync(ItemsFound, DataGridSourceConverter(operation.Result));
                }

                await ItemsFoundFillComplete();

                if (ItemsFound.Count == 1 && SelectIfOnlyOneItemReturned)
                    SelectedItem = ItemsFound.First();

                OnSearchComplete(operation);
            });

        protected abstract TSearchOrder DataCollectorConverter(TDataCollector dataCollector);

        protected virtual Task ItemsFoundFillComplete() => Task.CompletedTask;

        protected virtual Task ItemsFoundToBeCleared(IEnumerable<TDataGridSource> oldItems) => Task.CompletedTask;

        protected virtual Task ItemsFoundClearedBeforeSearch() => Task.CompletedTask;

        protected abstract IEnumerable<TDataGridSource> DataGridSourceConverter(IEnumerable<TSearchReturn> source);

        protected override void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    ItemsFound.CollectionChanged -= ItemsFound_CollectionChanged;
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }
    }
}
