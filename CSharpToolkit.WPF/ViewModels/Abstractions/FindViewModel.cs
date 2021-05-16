namespace CSharpToolkit.ViewModels.Abstractions {
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using CSharpToolkit.Extensions;
    using CSharpToolkit.Utilities;
    using CSharpToolkit.Utilities.Abstractions;
    using CSharpToolkit.Utilities.EventArgs;
    using CSharpToolkit.Validation.Abstractions;
    using CSharpToolkit.XAML;

    /// <summary>
    /// The abstract base class for the locators.
    /// </summary>
    /// <typeparam name="TDataGridSource">The type that represents what is being presented by the selector. Must implement <see cref="IIdProvider"/>.</typeparam>
    /// <typeparam name="TDataCollector">The type that represents what will hold the data being gathered.</typeparam>
    public abstract class FindViewModel<TDataGridSource, TDataCollector> : EntityBase, ILocator where TDataGridSource : IIdProvider {

        bool _disposed = false;
        int _constraint;
        Locker _formLocker = new Locker();
        Locker _clearingLocker = new Locker();
        TDataGridSource _selectedItem;
        IValidate<string> _validator;
        CSharpToolkit.XAML.Behaviors.TwoListSynchronizer _synchronizer;
        private SearchStatus _status;

        /// <summary>
        /// Instantiates the <see cref="FindViewModel{TDataGridSource, TDataCollector}"/> class.
        /// </summary>
        /// <param name="validator">The validator used.</param>
        /// <param name="dataCollectorInitializer">A function which initializes the <see cref="DataCollector"/>.</param>
        public FindViewModel(IValidate<string> validator, Func<TDataCollector> dataCollectorInitializer) {
            Validator = validator;

            Search = new AwaitableDelegateCommand(PrivateSearchCallback, SearchEnable);
            Clear = new AwaitableDelegateCommand(PrivateClearCallback, ClearEnable);
            RecordRequest = new AwaitableDelegateCommand(RecordRequestCallback, () => true);

            _formLocker.LockStatusChanged += (s, e) => OnPropertyChanged(nameof(Disable));
            _formLocker.LockStatusChanged += (s, e) => OnDisableChange?.Invoke(this, new GenericEventArgs<bool>(Disable));

            _clearingLocker.LockStatusChanged += (s, e) => OnPropertyChanged(nameof(Clearing));

            DataCollector = dataCollectorInitializer();

            ResetStringProperties();

            var currentItems = new ObservableCollection<IIdProvider>();
            CurrentItems = currentItems;

            _synchronizer = new XAML.Behaviors.TwoListSynchronizer(ItemsFound, currentItems, new XAML.Behaviors.CallbackListItemConverter(null, ConvertFromCurrentItemsToItemsFound));
            _synchronizer.StartSynchronizing();
        }


        private Task RecordRequestCallback() {
            RecordRequested?.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Used to validate input.
        /// </summary>
        /// <param name="columnName">The name of the property being validated.</param>
        /// <returns><see cref="System.String.Empty"/></returns>
        public override string this[string columnName] {
            get {
                ClearErrors(columnName);
                PropertyInfo info = GetType().GetProperty(columnName);
                if ((info?.PropertyType ?? typeof(object)) != typeof(string))
                    return string.Empty;

                OperationResult validationResult = Validator.Validate((string)info.GetValue(this));
                if (validationResult.HadErrors)
                    AddError(columnName, validationResult.Exceptions[0].Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// The <see cref="Clear"/> command.
        /// </summary>
        public AwaitableDelegateCommand Clear { get; }

        /// <summary>
        /// The <see cref="RecordRequest"/> command.
        /// </summary>
        public AwaitableDelegateCommand RecordRequest { get; }

        public event EventHandler<GenericEventArgs<bool>> OnDisableChange;

        /// <summary>
        /// The Id used to constrain the Locator.
        /// </summary>
        public int Constraint {
            get { return _constraint; }
            set {
                int oldValue = _constraint;
                if (Perform.ReplaceIfDifferent(ref _constraint, value).WasSuccessful) {
                    OnConstraintChanged(oldValue);
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The currently items of the instance.
        /// </summary>
        public IList<IIdProvider> CurrentItems { get; }

        /// <summary>
        /// The object used to gather information.
        /// </summary>
        public TDataCollector DataCollector { get; }

        /// <summary>
        /// Whether or not the ViewModel is completely disabled.
        /// </summary>
        public bool Disable => _formLocker.IsLocked();

        /// <summary>
        /// Whether or not the ViewModel is currently clearing.
        /// </summary>
        public bool Clearing => _clearingLocker.IsLocked();

        /// <summary>
        /// The items procured from the database.
        /// </summary>
        public ObservableCollection<TDataGridSource> ItemsFound { get; } = new ObservableCollection<TDataGridSource>();

        /// <summary>
        /// A collection to display color meanings on items.
        /// </summary>
        public ObservableCollection<ColorLegend> ColorLegend { get; } = new ObservableCollection<ColorLegend>();

        /// <summary>
        /// A collection to display icon meanings on items.
        /// </summary>
        public ObservableCollection<IconLegend> IconLegend { get; } = new ObservableCollection<IconLegend>();

        /// <summary>
        /// The <see cref="Search"/> command.
        /// </summary>
        public AwaitableDelegateCommand Search { get; }

        /// <summary>
        /// A restrictor that only will allow the selected item to exist in the <see cref="SelectedItems"/> collection.
        /// </summary>
        public TDataGridSource SelectedItem {
            get { return _selectedItem; }
            set {
                if (Perform.ReplaceIfDifferent(ref _selectedItem, value).WasSuccessful) {
                    OnSelectedItemChanged();
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The current selected items.
        /// </summary>
        public ObservableCollection<IIdProvider> SelectedItems { get; } = new ObservableCollection<IIdProvider>();

        /// <summary>
        /// The validator used to validate the information gathered.
        /// </summary>
        public IValidate<string> Validator {
            get { return _validator ?? CSharpToolkit.Validation.NullValidator<string>.Instance; }
            set { _validator = value; }
        }

        /// <summary>
        /// The current search state of the instance.
        /// </summary>
        public SearchStatus SearchStatus {
            get { return _status; }
            private set { FirePropertyChangedIfDifferent(ref _status, value); }
        }

        /// <summary>
        /// The <see cref="Cleared"/> event. This event is normally fired when the clear command executes.
        /// </summary>
        public event EventHandler Cleared;

        /// <summary>
        /// The <see cref="ExportId"/> event. This event is used when an Id should be exported from the find to listeners.
        /// </summary>
        public event EventHandler<GenericEventArgs<int>> ExportId;

        /// <summary>
        /// The <see cref="SearchComplete"/> event. Signals that a search operation has just completed. The parameter details whether the search was successful, or had errors.
        /// </summary>
        public event EventHandler<GenericEventArgs<OperationResult>> SearchComplete;

        /// <summary>
        /// The <see cref="SearchToBegin"/> event. Signals that a search operation has begun.
        /// </summary>
        public event EventHandler SearchToBegin;

        /// <summary>
        /// The <see cref="RecordRequested"/> event. Used by FindViewModel when a single record has been requested, most likely by a double click.
        /// </summary>
        public event EventHandler RecordRequested;

        /// <summary>
        /// Used to refresh the collections on the ViewModel.
        /// </summary>
        /// <returns>Task used for asynchronous call.</returns>
        public abstract Task RefreshCollections();

        /// <summary>
        /// Used to fire the <see cref="Clear"/> command's callback.
        /// </summary>
        public void RequestClear() =>
            ClearCallback();

        /// <summary>
        /// Attempts to remove item from instance.
        /// </summary>
        /// <param name="item">Item to remove.</param>
        public void Remove(IIdProvider item) {
            if (item is TDataGridSource)
                ItemsFound.Remove((TDataGridSource)item);
        }

        /// <summary>
        /// Used to request a disable.
        /// </summary>
        /// <param name="token">The lock token. The same token must be passed when requesting an unlock.</param>
        public void RequestDisable(object token) => _formLocker.RequestLock(token);

        /// <summary>
        /// Used to request an enable.
        /// </summary>
        /// <param name="token">The lock token. The same token must have been passed when requesting lock.</param>
        public void RequestEnable(object token) => _formLocker.RequestUnlock(token);

        /// <summary>
        /// Used to fire the <see cref="Search"/> command's callback.
        /// </summary>
        public virtual Task RequestSearch() =>
            SearchCallback();

        /// <summary>
        /// The callback for the <see cref="Clear"/> command.
        /// </summary>
        /// <returns>Task to run asynchronously.</returns>
        protected virtual Task ClearCallback() {
            ResetStringProperties();
            SelectedItems.Clear();
            ItemsFound.Clear();
            return Task.CompletedTask;
        }

        /// <summary>
        /// The callback that determines if <see cref="Clear"/> command is enabled.
        /// </summary>
        /// <returns>True dictates command can run. False disables it.</returns>
        protected virtual bool ClearEnable() =>
           _formLocker.IsFree();

        /// <summary>
        /// A helper method that will lock the form during execution of a method.
        /// </summary>
        /// <param name="method">The task during which the instance will be locked.</param>
        /// <returns>A task for awaiting.</returns>
        protected Task LockFormForExecution(Func<Task> method) =>
            _formLocker.LockForDurationAsync(method);

        /// <summary>
        /// A helper method that will lock the form during execution of a method.
        /// </summary>
        /// <param name="method">The task during which the instance will be locked.</param>
        /// <returns>A task for awaiting.</returns>
        protected void LockFormForExecution(Action method) =>
            _formLocker.LockForDuration(method);

        /// <summary>
        /// Used to fire <see cref="Cleared"/> event. Normally occurs when <see cref="Clear"/> has finished, but <see cref="Clearing"/> flag is still set.
        /// </summary>
        protected void OnCleared() =>
            Cleared?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Logic to be run when <see cref="Constraint"/> changes.
        /// </summary>
        /// <param name="oldValue">The previous value.</param>
        protected virtual void OnConstraintChanged(int oldValue) { }

        /// <summary>
        /// Used to fire the <see cref="ExportId"/> event.
        /// </summary>
        /// <param name="id">The Id to export.</param>
        protected void OnExportId(int id) =>
            ExportId?.Invoke(this, new GenericEventArgs<int>(id));

        /// <summary>
        /// Used to fire the <see cref="SearchComplete"/> event.
        /// </summary>
        protected void OnSearchComplete(OperationResult result) =>
            SearchComplete?.Invoke(this, new GenericEventArgs<OperationResult>(result));

        /// <summary>
        /// Used to fire the <see cref="SearchToBegin"/> event.
        /// </summary>
        protected void OnSearchToBegin() =>
            SearchToBegin?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Logic to be run when <see cref="SelectedItem"/> changes. By default, fires the <see cref="ExportId"/> event with <see cref="SelectedItem.Id"/>.
        /// </summary>
        protected virtual void OnSelectedItemChanged() {
            int id = SelectedItem?.Id ?? 0;
            if (id != 0)
                OnExportId(id);
        }

        /// <summary>
        /// Resets all string properties on the object to String.Empty.
        /// </summary>
        protected virtual void ResetStringProperties() {
            GetType()
                .GetProperties()
                .Where(p => p.PropertyType == typeof(string) && p.CanWrite && p.SetMethod.IsPublic)
                .ForEach(p => p.SetValue(this, ""));
        }

        /// <summary>
        /// The callback for the <see cref="Search"/> command.
        /// </summary>
        /// <returns>Task to run asynchronously.</returns>
        protected abstract Task SearchCallback();

        /// <summary>
        /// The callback that determines if <see cref="Search"/> command is enabled.
        /// </summary>
        /// <returns>True dictates command can run. False disables it.</returns>
        protected virtual bool SearchEnable() =>
            ((HasErrors || Disable) == false)
                && SearchStatus == SearchStatus.Idle
                && Clearing == false;

        protected abstract object ConvertFromCurrentItemsToItemsFound(object arg);

        private async Task PrivateClearCallback() {
            await _clearingLocker.LockForDurationAsync(async () => {
                await ClearCallback();
                OnCleared();
            });
        }

        public void ClearSelectedItem() {
            SelectedItem = default(TDataGridSource);
        }

        private async Task PrivateSearchCallback() {
            SearchStatus = SearchStatus.Searching;
            await SearchCallback();
            SearchStatus = SearchStatus.Idle;
        }

        protected override void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    OnDisableChange = null;
                    SearchComplete = null;
                    SearchToBegin = null;
                    ExportId = null;
                    Cleared = null;
                    ItemsFound.Clear();
                    SelectedItems.Clear();
                    _formLocker.Dispose();
                    _synchronizer.StopSynchronizing();
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }

    }
}
