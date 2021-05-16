namespace CSharpToolkit.Utilities {
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using CSharpToolkit.Extensions;
    using CSharpToolkit.Utilities.Abstractions;
    using CSharpToolkit.Utilities.EventArgs;

    public class ModifyableItemsOnlyLocator<T> : IVMLocator<T>, IDisposable where T : IIdProvider {

        bool _disposed = false;
        List<T> _modifyableItems = new List<T>();
        List<IIdProvider> _currentItems = new List<IIdProvider>();
        CSharpToolkit.XAML.Behaviors.TwoListSynchronizer _synchronizer;

        public ModifyableItemsOnlyLocator(IEnumerable<T> modifyableItems, Func<object, object> convertFromIIdProviderToT) : this(modifyableItems, Enumerable.Empty<IIdProvider>(), convertFromIIdProviderToT)  { }
        public ModifyableItemsOnlyLocator(IEnumerable<T> modifyableItems, IEnumerable<IIdProvider> selectedItems, Func<object, object> convertFromIIdProviderToT) {

            _modifyableItems = new List<T>(modifyableItems ?? new T[0]);
            _currentItems = new List<IIdProvider>();
            selectedItems.ForEach(item => SelectedItems.Add(item));
            _synchronizer = new CSharpToolkit.XAML.Behaviors.TwoListSynchronizer(_modifyableItems, _currentItems, new CSharpToolkit.XAML.Behaviors.CallbackListItemConverter(null, convertFromIIdProviderToT));
            _synchronizer.StartSynchronizing();
        }

        public int Constraint { get; set; }

        public IList<IIdProvider> CurrentItems => _currentItems;
        public IList<T> ModifyableItems => _modifyableItems;
        public ObservableCollection<IIdProvider> SelectedItems { get; } = new ObservableCollection<IIdProvider>();

        public event EventHandler Cleared;
        public event EventHandler RecordRequested;
        public event EventHandler<GenericEventArgs<int>> ExportId;
        public event EventHandler SearchToBegin;
        public event EventHandler<GenericEventArgs<OperationResult>> SearchComplete;

        public void ClearSelectedItem() { }

        public Task RefreshCollections() => Task.CompletedTask;

        public void Remove(IIdProvider item) =>
            _modifyableItems.Where(mItem => mItem.Id == item.Id).ToArray().ForEach(mItem => _modifyableItems.Remove(mItem));

        public void RequestClear() { }

        public void RequestDisable(object token) { }

        public void RequestEnable(object token) { }

        public Task RequestSearch() => Task.CompletedTask;

        public void Dispose() => Dispose(true);

        protected void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    Cleared = null;
                    RecordRequested = null;
                    ExportId = null;
                    SearchComplete = null;
                    SearchToBegin = null;
                    _synchronizer.StopSynchronizing();
                }
            }
        }
    }
}