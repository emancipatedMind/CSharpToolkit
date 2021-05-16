namespace CSharpToolkit.ViewModels.Abstractions {
    using System;
    using System.Collections.Generic;
    using CSharpToolkit.Utilities.Abstractions;
    using CSharpToolkit.Validation.Abstractions;
    using System.Linq;
    using CSharpToolkit.Utilities;
    using System.Collections.ObjectModel;

    /// <summary>
    /// The abstract base class for the locators who has the ability for its collections to be modified.
    /// </summary>
    /// <typeparam name="TDataGridSource">The type that represents what is being presented by <see cref="FindViewModel{TDataGridSource, TDataCollector}.ItemsFound"/>, and <see cref="FindViewModel{TDataGridSource, TDataCollector}.SelectedItem"/>. Must implement <see cref="IIdProvider"/>, and <typeparamref name="TModifyable"/>.</typeparam>
    /// <typeparam name="TDataCollector">The type that represents what will hold the data being gathered.</typeparam>
    /// <typeparam name="TModifyable">The type that represents what will be modifyable. Must implement <see cref="IIdProvider"/>.</typeparam>
    public abstract class ModifyableFindViewModel<TDataGridSource, TDataCollector, TModifyable> : FindViewModel<TDataGridSource, TDataCollector>, IVMLocator<TModifyable> where TDataGridSource : IIdProvider, TModifyable where TModifyable : IIdProvider {

        bool _disposed;
        CSharpToolkit.XAML.Behaviors.TwoListSynchronizer _synchronizer;

        /// <summary>
        /// Instantiates <see cref="ModifyableFindViewModel{TDataGridSource, TDataCollector, TModifyable}"/>.
        /// </summary>
        /// <param name="validator">The validator used.</param>
        /// <param name="dataCollectorInitializer">A function which initializes the <see cref="DataCollector"/>.</param>
        public ModifyableFindViewModel(IValidate<string> validator, Func<TDataCollector> dataCollectorInitializer) : base(validator, dataCollectorInitializer) {

            var modifyableItems = new ObservableCollection<TModifyable>();
            ModifyableItems = modifyableItems;

            _synchronizer = new CSharpToolkit.XAML.Behaviors.TwoListSynchronizer(ItemsFound, modifyableItems, new CSharpToolkit.XAML.Behaviors.CallbackListItemConverter(x => x, ConvertFromModifyableToItemsFound));
            _synchronizer.StartSynchronizing();
        }

        /// <summary>
        /// The collection of items that are modifyable.
        /// </summary>
        public IList<TModifyable> ModifyableItems { get; }

        protected abstract object ConvertFromModifyableToItemsFound(object arg);

        protected override void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    _synchronizer.StopSynchronizing();
                } 
                _disposed = true;
            } 
            base.Dispose(disposing);
        }


    }
}
