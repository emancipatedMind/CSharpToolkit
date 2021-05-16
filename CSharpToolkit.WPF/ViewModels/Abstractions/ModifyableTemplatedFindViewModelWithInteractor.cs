namespace CSharpToolkit.ViewModels.Abstractions {
    using System;
    using System.Threading.Tasks;
    using CSharpToolkit.Utilities.Abstractions;
    using CSharpToolkit.Validation.Abstractions;
    /// <summary>
    /// An implementation of the <see cref="ModifyableTemplatedFindViewModel{TDataGridSource, TDataCollector, TModifyable, TSearchOrder, TSearchReturn}"/> that includes an interactor that controls whether or not the search can be performed.
    /// </summary>
    /// <typeparam name="TDataGridSource">The type that represents what is being presented by <see cref="FindViewModel{TDataGridSource, TDataCollector}.ItemsFound"/>, and <see cref="FindViewModel{TDataGridSource, TDataCollector}.SelectedItem"/>. Must implement <see cref="IIdProvider"/>, and <typeparamref name="TModifyable"/>.</typeparam>
    /// <typeparam name="TDataCollector">The type that represents what will hold the data being gathered.</typeparam>
    /// <typeparam name="TModifyable">The type that represents what will be modifyable. Must implement <see cref="IIdProvider"/>.</typeparam>
    /// <typeparam name="TSearchOrder">The type that represents what is used by the DataAccessor for transmission of search criteria.</typeparam>
    /// <typeparam name="TSearchReturn">The type that represents what is used by the DataAccessor for transmission of search results.</typeparam>
    public abstract class ModifyableTemplatedFindViewModelWithInteractor<TDataGridSource, TDataCollector, TModifyable, TSearchOrder, TSearchReturn> 
        : ModifyableTemplatedFindViewModel<TDataGridSource, TDataCollector, TModifyable, TSearchOrder, TSearchReturn> where TDataGridSource : TModifyable where TModifyable : IIdProvider {
        ISimpleFindInteractor<TDataCollector, TSearchOrder, TSearchReturn> _interactor;

        public ModifyableTemplatedFindViewModelWithInteractor(ISimpleFindInteractor<TDataCollector, TSearchOrder, TSearchReturn> interactor, IValidate<string> validator, Func<TDataCollector> dataCollectorInitializer) : base(interactor, validator, dataCollectorInitializer) {
            _interactor = interactor;
        }

        protected override bool SearchEnable() =>
            base.SearchEnable() && _interactor.AllowSearch(DataCollector);

        public override Task RequestSearch() =>
            _interactor.AllowSearch(DataCollector) ?
                base.RequestSearch() :
                Task.CompletedTask;

    }
}
