namespace CSharpToolkit.ViewModels.Abstractions {
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using CSharpToolkit.Extensions;
    using CSharpToolkit.Utilities;
    using CSharpToolkit.Utilities.Abstractions;
    using CSharpToolkit.Utilities.EventArgs;
    using CSharpToolkit.XAML;
    using CSharpToolkit.ViewModels.Abstractions;
    public abstract class AdminViewModel<TViewModel, TModel, TModelSearcher> : SideBarDialogWindowViewModel<EventArgs> where TViewModel : EditableBase<TModel>, TModel, new() {
        static Func<bool, bool> DefaultEnableCallback = new Func<bool, bool>(b => b);

        bool _isDependent = false;
        bool _disposedValue = false;

        IVMLocator<TModelSearcher> _modelSearcher;
        CriticalOperationType _criticalOperationType;
        List<object> locatorsRecordRequested = new List<object>();

        Func<bool, bool> _newEnableCallback;
        Func<bool, bool> _duplicateEnableCallback;
        Func<bool, bool> _modifyEnableCallback;
        Func<bool, bool> _backEnableCallback;
        Func<bool, bool> _forwardEnableCallback;
        Func<bool, bool> _performDeleteOnRecordEnableCallback;
        IExtendedNavigator<int> _backForwardNavigator;

        protected object _adminToken = new object();
        object _modifyingToken = new object();

        public AdminViewModel() {
            Navigation.New = new AwaitableDelegateCommand(() => ConfirmCancelLocker.LockForDurationAsync(NewCallback), () => NewEnableCallback(NewEnable()));
            Navigation.Duplicate = new AwaitableDelegateCommand(() => ConfirmCancelLocker.LockForDurationAsync(DuplicateCallback), () => DuplicateEnableCallback(DuplicateEnable()));
            Navigation.Modify = new AwaitableDelegateCommand(() => ConfirmCancelLocker.LockForDurationAsync(ModifyCallback), () => ModifyEnableCallback(ModifyEnable()));

            Navigation.ReloadCurrent = new AwaitableDelegateCommand(() => RefreshModelInformation(Model.Id), () => ReloadCurrentEnable());

            Navigation.Confirm = new AwaitableDelegateCommand(() => ConfirmCancelLocker.LockForDurationAsync(ConfirmCallback), () => ConfirmEnable());
            Navigation.Cancel = new AwaitableDelegateCommand(() => ConfirmCancelLocker.LockForDurationAsync(CancelCallback), () => CancelEnable());

            Model.ErrorsChanged += Model_ErrorsChanged;

            _backForwardNavigator = new ExtendedNavigator<int>();
            SetUpNewNavigator();

            ConfirmCancelLocker.LockStatusChanged += (s, e) => {
                Navigation.New.RaiseCanExecuteChanged();
                Navigation.Duplicate.RaiseCanExecuteChanged();
                Navigation.Modify.RaiseCanExecuteChanged();
                Navigation.Confirm.RaiseCanExecuteChanged();
                Navigation.Cancel.RaiseCanExecuteChanged();
            };

            NewRecordCancelLocker.LockStatusChanged += (s, e) => {
                Navigation.Confirm.RaiseCanExecuteChanged();
                Navigation.Cancel.RaiseCanExecuteChanged();
            };

            OutsideModificationLocker.RequestLock(_modifyingToken);
            OutsideModificationLocker.LockStatusChanged += (s, e) => OnPropertyChanged(nameof(OutsideModificationAllowed));  

        }

        /// <summary>
        /// The model to be managed by the <see cref="AdminViewModel{TViewModel, TModel, TModelSearcher}" />.
        /// </summary>
        public TViewModel Model { get; } = new TViewModel();
        /// <summary>
        /// The list of Locators for this admin.
        /// </summary>
        protected List<ILocator> Locators { get; } = new List<ILocator>();

        /// <summary>
        /// The ViewModel for the Navigation commands.
        /// </summary>
        public NavigationalViewModel Navigation { get; } = new NavigationalViewModel();

        /// <summary>
        /// The instance responsible for the back, and forth logic.
        /// </summary>
        protected IExtendedNavigator<int> BackForwardNavigator => _backForwardNavigator;

        /// <summary>
        /// Changes value of <see cref="OutsideModificationAllowed"/>. When locked, <see cref="OutsideModificationAllowed"/> is false;
        /// </summary>
        public ILocker OutsideModificationLocker { get; } = new Locker();

        /// <summary>
        /// Override for Navigation enable. Disallows record change.
        /// </summary>
        public ILocker RecordChangeLocker { get; } = new Locker();

        /// <summary>
        /// Override for the confirm, and cancel enable.
        /// </summary>
        public ILocker ConfirmCancelLocker { get; } = new Locker();

        /// <summary>
        /// Override for the cancel enable that will only lock if adding a new record.
        /// </summary>
        public ILocker NewRecordCancelLocker { get; } = new Locker();

        /// <summary>
        /// Override for Navigation. Will disable all buttons.
        /// </summary>
        public ILocker NavigationalOverride { get; } = new Locker();

        /// <summary>
        /// Used to swap the <see cref="Navigation" /> that is used for the back, and forth logic.
        /// </summary>
        /// <param name="newNavigator">The new <see cref="Navigation" />.</param>
        /// <returns>Task for async use.</returns>
        public async Task SwapNavigator(IExtendedNavigator<int> newNavigator) {
            var oldValue = _backForwardNavigator;
            if (Perform.ReplaceIfDifferent(ref _backForwardNavigator, newNavigator, new ExtendedNavigator<int>()).WasSuccessful) {
                await BackForwardNavigatorSwapped(_backForwardNavigator, oldValue);
            }
        }

        /// <summary>
        /// The logic used to swap the <see cref="Navigation" />.
        /// </summary>
        /// <param name="newValue">The new <see cref="Navigation" />.</param>
        /// <param name="oldValue">The old <see cref="Navigation" /></param>
        /// <returns>Task for async use.</returns>
        protected virtual async Task BackForwardNavigatorSwapped(IExtendedNavigator<int> newValue, IExtendedNavigator<int> oldValue) {
            if (oldValue != null) {
                await newValue.UpdateCurrentItem(oldValue.CurrentItem);
                oldValue.CurrentItemChangedCallback = null;
            }
            SetUpNewNavigator();
        }

        /// <summary>
        /// The logic for setting up the new <see cref="Navigation" />.
        /// </summary>
        protected virtual void SetUpNewNavigator() {
            _backForwardNavigator.CurrentItemChangedCallback = OnCurrentItemChanged;
            Navigation.Back = new AwaitableDelegateCommand(_backForwardNavigator.NavigateBack, () => BackEnableCallback(BackEnable()));
            Navigation.Forward = new AwaitableDelegateCommand(_backForwardNavigator.NavigateForward, () => ForwardEnableCallback(ForwardEnable()));
        }

        /// <summary>
        /// An enumeration of the dependent locators managed by the <see cref="AdminViewModel{TViewModel, TModel, TModelSearcher}" />. Does not include the main locator.
        /// </summary>
        public IEnumerable<ILocator> DependentLocators =>
            ModelSearcher == null ?
                Locators :
                Locators.Where(loc => ReferenceEquals(loc, ModelSearcher) == false);

        /// <summary>
        /// Used to determine whether the <see cref="AdminViewModel{TViewModel, TModel, TModelSearcher}" /> is currently hosted by another form.
        /// </summary>
        public bool IsDependent {
            get { return _isDependent; }
            set {
                if (Perform.ReplaceIfDifferent(ref _isDependent, value).WasSuccessful) {
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Whether the <see cref="AdminViewModel{TViewModel, TModel, TModelSearcher}" /> is currently modifying its <see cref="Model" />.
        /// </summary>
        public bool Modifying {
            get { return Model.Modifying; }
            set {
                if (Perform.ReplaceIfDifferent(Model, nameof(Model.Modifying), value).WasSuccessful) {
                    if (value) {
                        OutsideModificationLocker.RequestUnlock(_modifyingToken); 
                    } 
                    else {
                        OutsideModificationLocker.RequestLock(_modifyingToken); 
                    }
                    OnPropertyChanged();
                } 
            }
        }

        /// <summary>
        /// Whether the <see cref="AdminViewModel{TViewModel, TModel, TModelSearcher}" /> is currently modifying its <see cref="Model" />.
        /// </summary>
        public bool OutsideModificationAllowed => OutsideModificationLocker.IsFree();

        /// <summary>
        /// The <see cref="Model" /> to use when initiating a new record.
        /// </summary>
        protected TModel DefaultModel =>
            (DefaultModelCallback == null ? StoredDefault : DefaultModelCallback(StoredDefault));

        /// <summary>
        /// The callback to produce the <see cref="DefaultModel"/>. The parameter is <see cref="StoredDefault"/>. If null, the <see cref="StoredDefault"/> is used.
        /// </summary>
        public Func<TModel, TModel> DefaultModelCallback { get; set; }

        /// <summary>
        /// The main locator of the <see cref="AdminViewModel{TViewModel, TModel, TModelSearcher}" />.
        /// </summary>
        public IVMLocator<TModelSearcher> ModelSearcher => _modelSearcher;

        /// <summary>
        /// Used to determine whether the <see cref="AdminViewModel{TViewModel, TModel, TModelSearcher}" /> is adding a record, or is modifying a record.
        /// </summary>
        public CriticalOperationType CriticalOperationType {
            get { return _criticalOperationType; }
            set {
                var oldValue = _criticalOperationType;
                if (Perform.ReplaceIfDifferent(ref _criticalOperationType, value).WasSuccessful) {
                    OnPropertyChanged();
                    OnCriticalOperationTypeChanged(oldValue);
                }
            }
        }

        /// <summary>
        /// The enable method override for the New command of the <see cref="Navigation" />. The parameter is default enable evaluated.
        /// </summary>
        public Func<bool, bool> NewEnableCallback {
            get { return _newEnableCallback ?? DefaultEnableCallback; }
            set { _newEnableCallback = value; }
        }

        /// <summary>
        /// The enable method override for the Duplicate command of the <see cref="Navigation" />. The parameter is default enable evaluated.
        /// </summary>
        public Func<bool, bool> DuplicateEnableCallback {
            get { return _duplicateEnableCallback ?? DefaultEnableCallback; }
            set { _duplicateEnableCallback = value; }
        }

        /// <summary>
        /// The enable method override for the Modify command of the <see cref="Navigation" />. The parameter is default enable evaluated.
        /// </summary>
        public Func<bool, bool> ModifyEnableCallback {
            get { return _modifyEnableCallback ?? DefaultEnableCallback; }
            set { _modifyEnableCallback = value; }
        }

        /// <summary>
        /// The enable method override for the Back command of the <see cref="Navigation" />. The parameter is default enable evaluated.
        /// </summary>
        public Func<bool, bool> BackEnableCallback {
            get { return _backEnableCallback ?? DefaultEnableCallback; }
            set { _backEnableCallback = value; }
        }

        /// <summary>
        /// The enable method override for the Forward command of the <see cref="Navigation" />. The parameter is default enable evaluated.
        /// </summary>
        public Func<bool, bool> ForwardEnableCallback {
            get { return _forwardEnableCallback ?? DefaultEnableCallback; }
            set { _forwardEnableCallback = value; }
        }

        /// <summary>
        /// The enable method override for the Cascading Delete command of the Side Bar.
        /// </summary>
        public Func<bool, bool> PerformDeleteOnRecordEnableCallback {
            get { return _performDeleteOnRecordEnableCallback ?? DefaultEnableCallback; }
            set { _performDeleteOnRecordEnableCallback = value; }
        }

        /// <summary>
        /// The default <see cref="Model" /> stored inside of the <see cref="AdminViewModel{TViewModel, TModel, TModelSearcher}" />.
        /// </summary>
        public abstract TModel StoredDefault { get; }

        /// <summary>
        /// The logic used to refresh the <see cref="Model" />.
        /// </summary>
        /// <param name="id">The id of the record.</param>
        /// <returns>Task for async use.</returns>
        public abstract Task RefreshModelInformation(int id);

        /// <summary>
        /// The logic used to refresh any collections managed by the <see cref="AdminViewModel{TViewModel, TModel, TModelSearcher}" />.
        /// </summary>
        /// <returns></returns>
        public abstract Task RefreshCollections();

        /// <summary>
        /// The default enable for the New command;
        /// </summary>
        /// <returns>True enables button. False is a disable.</returns>
        protected virtual bool NewEnable() =>
            this.StateNotCriticalEnable() && this.StateNotRestrictedEnable() && RecordChangeLocker.IsFree() && NavigationalOverride.IsFree();

        /// <summary>
        /// The default enable for the Duplicate command;
        /// </summary>
        /// <returns>True enables button. False is a disable.</returns>
        protected virtual bool DuplicateEnable() =>
            this.ModelValidEnable() && this.StateNotCriticalEnable() && this.StateNotRestrictedEnable() && RecordChangeLocker.IsFree() && NavigationalOverride.IsFree();

        protected override bool PerformDeleteOnRecordEnable() =>
            PerformDeleteOnRecordEnableCallback(DefaultPerformDeleteOnRecordEnable());

        /// <summary>
        /// The default enable for the Cascading Delete command.
        /// </summary>
        /// <returns>True enables button. False is a disable.</returns>
        protected bool DefaultPerformDeleteOnRecordEnable() =>
            this.ModelValidEnable() && this.StateNotCriticalEnable() && this.StateNotRestrictedEnable() && RecordChangeLocker.IsFree();

        /// <summary>
        /// The logic run for the New Command.
        /// </summary>
        /// <returns>Task for async use.</returns>
        protected abstract Task NewCallback();
        /// <summary>
        /// The logic run for the Duplicate Command.
        /// </summary>
        /// <returns>Task for async use.</returns>
        protected abstract Task DuplicateCallback();

        /// <summary>
        /// The default enable for the Modify Command.
        /// </summary>
        /// <returns>True enables button. False is a disable.</returns>
        protected virtual bool ModifyEnable() =>
            this.ModelValidEnable() && this.StateNotCriticalEnable() && this.StateNotRestrictedEnable() && NavigationalOverride.IsFree();

        /// <summary>
        /// The default enable for the Back Command.
        /// </summary>
        /// <returns>True enables button. False is a disable.</returns>
        protected virtual bool BackEnable() =>
            _backForwardNavigator.CanNavigateBack() && this.StateNotCriticalEnable() && RecordChangeLocker.IsFree() && NavigationalOverride.IsFree();

        /// <summary>
        /// The default enable for the Forward Command.
        /// </summary>
        /// <returns>True enables button. False is a disable.</returns>
        protected virtual bool ForwardEnable() =>
            _backForwardNavigator.CanNavigateForward() && this.StateNotCriticalEnable() && RecordChangeLocker.IsFree() && NavigationalOverride.IsFree();

        /// <summary>
        /// The default logic for the Modify Command.
        /// </summary>
        /// <returns>Task for async use.</returns>
        protected abstract Task ModifyCallback();

        /// <summary>
        /// The default enable for the ReloadCurrent Command.
        /// </summary>
        /// <returns>True enables button. False is a disable.</returns>
        protected virtual bool ReloadCurrentEnable() =>
            this.ModelValidEnable() && this.StateNotCriticalEnable() && NavigationalOverride.IsFree();

        /// <summary>
        /// The default enable for the Cancel Command.
        /// </summary>
        /// <returns>True enables button. False is a disable.</returns>
        protected virtual bool CancelEnable() =>
            Modifying && ConfirmCancelLocker.IsFree() && (NewRecordCancelLocker.IsFree() || CriticalOperationType != CriticalOperationType.AddingNewRecord) && NavigationalOverride.IsFree();
        /// <summary>
        /// The default logic for the Cancel Command.
        /// </summary>
        /// <returns>Task for async use.</returns>
        protected abstract Task CancelCallback();

        /// <summary>
        /// The default enable for the Confirm Command.
        /// </summary>
        /// <returns>True enables button. False is a disable.</returns>
        protected virtual bool ConfirmEnable() =>
            HasErrors == false && Modifying && ConfirmCancelLocker.IsFree() && NavigationalOverride.IsFree();

        /// <summary>
        /// The logic for the Confirm Command.
        /// </summary>
        /// <returns>Task for async use.</returns>
        protected abstract Task ConfirmCallback();

        /// <summary>
        /// The logic used whenever a dependent locator is swapped.
        /// </summary>
        /// <param name="newLocator">The new locator.</param>
        /// <param name="oldLocator">The old locator.</param>
        /// <param name="token">The token to use to lock.</param>
        protected void ProcessLocatorSwap(ILocator newLocator, ILocator oldLocator, object token) {
            if (oldLocator != null) {
                Locators.Remove(oldLocator);
                oldLocator.SearchToBegin -= Locator_SearchToBegin;
                oldLocator.SearchComplete -= Locator_SearchComplete;
                oldLocator.RecordRequested -= Locator_RecordRequested;
            }
            if (newLocator != null) {
                Locators.Add(newLocator);
                newLocator.RequestClear();
                newLocator.RequestDisable(token);
                newLocator.SearchToBegin += Locator_SearchToBegin;
                newLocator.SearchComplete += Locator_SearchComplete;
                newLocator.RecordRequested += Locator_RecordRequested;
            }
        }

        private void SelectedItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) =>
            OnModelSearcherSelectedItemsChanged(e);

        /// <summary>
        /// The logic for use when the selected items of the <see cref="ModelSearcher" /> change.
        /// </summary>
        /// <param name="e">The EventArgs for the instance.</param>
        protected virtual void OnModelSearcherSelectedItemsChanged(NotifyCollectionChangedEventArgs e) {
            var firstTwoCount = ModelSearcher.SelectedItems.Take(2).Count();

            if (firstTwoCount == 2) {
                var currentItems = ModelSearcher.CurrentItems.ToList();
                var selectedItems = ModelSearcher.SelectedItems.OrderBy(item => currentItems.IndexOf(item)).Select(item => item.Id);
                _backForwardNavigator.UpdateNavigationCollection(selectedItems);
                if (selectedItems.Contains(_backForwardNavigator.CurrentItem) == false)
                    _backForwardNavigator.UpdateCurrentItem(selectedItems.First());
            }
            else {
                _backForwardNavigator.UpdateNavigationCollection(ModelSearcher.CurrentItems.Select(item => item.Id));

                // Small quirk where if a bunch of items are selected, and then navigated through, re-selecting
                // the first item in that list does not trigger a refresh because technically, that first item
                // was the old selected item, and the new selected item. Refreshes are only triggered when the item changes.
                // This next piece of code remedies that.
                if (e.Action == NotifyCollectionChangedAction.Remove && firstTwoCount == 1)
                    _backForwardNavigator.UpdateCurrentItem(ModelSearcher.SelectedItems[0].Id);
            }
        }

        /// <summary>
        /// The logic for used when the <see cref="ModelSearcher" /> is swapped.
        /// </summary>
        /// <param name="oldModelSearcher">The old <see cref="ModelSearcher" />.</param>
        /// <returns>Task for async use.</returns>
        protected virtual Task OnModelSearcherSwapped(ILocator oldModelSearcher) => Task.CompletedTask;

        /// <summary>
        /// The EventHandler for the <see cref="ModelSearcher" /> ExportId event.
        /// </summary>
        /// <param name="sender">The initiator of the event.</param>
        /// <param name="e">The EventArgs containing the new Id.</param>
        private void ModelSearcher_ExportId(object sender, GenericEventArgs<int> e) {
            if (CriticalOperationType == CriticalOperationType.None) {
                OnModelSearcherExportId(e.Data);
            }
            else {
                ModelSearcher.ClearSelectedItem();
            }
        }

        /// <summary>
        /// The logic to be run if <see cref="ModelSearcher" />'s export is accepted.
        /// </summary>
        /// <param name="id">The id exported.</param>
        /// <returns>Task for async use.</returns>
        protected virtual Task OnModelSearcherExportId(int id) =>
            _backForwardNavigator.UpdateCurrentItem(id);

        /// <summary>
        /// The logic to run when the <see cref="ModelSearcher" /> is cleared.
        /// </summary>
        protected virtual void OnModelSearcherCleared() {
            _backForwardNavigator.UpdateCurrentItem(-1);
            _backForwardNavigator.UpdateNavigationCollection(new int[0]);
            DependentLocators.ForEach(locator => locator.RequestDisable(_adminToken));
            Model.Clear();
            ClearAndDisableDependentLocators(_adminToken);
        }

        /// <summary>
        /// The EventHandler for the <see cref="ModelSearcher" /> Cleared event.
        /// </summary>
        /// <param name="sender">The initiator of the event.</param>
        /// <param name="e">The EventArgs for the event.</param>
        private void ModelSearcher_Cleared(object sender, EventArgs e) =>
            OnModelSearcherCleared();

        /// <summary>
        /// The EventHandler for the <see cref="Model" /> ErrorsChanged event.
        /// </summary>
        /// <param name="sender">The initiator of the event.</param>
        /// <param name="e">The EventArgs containing the data errors found by the <see cref="Model" />.</param>
        private void Model_ErrorsChanged(object sender, System.ComponentModel.DataErrorsChangedEventArgs e) {
            ClearErrors(nameof(Model));
            if (Model.HasErrors) {
                var errors = Model.GetErrors("").Cast<IList<string>>();
                AddErrors(nameof(Model), errors.SelectMany(er => er));
            }
        }

        /// <summary>
        /// The logic for use by the <see cref="Navigation" />.
        /// </summary>
        /// <param name="currentItem">The new Id.</param>
        /// <returns>Task for async use.</returns>
        private async Task OnCurrentItemChanged(int currentItem) {
            // This code is to ensure that the ModelSearcher stays in sync with the navigator.
            if (ModelSearcher?.SelectedItems.Take(2).Count() == 1) {
                var selectedItem = ModelSearcher?.CurrentItems.FirstOrDefault(item => item.Id == currentItem);
                if (selectedItem != null) {
                    ModelSearcher?.SelectedItems.Clear();
                    ModelSearcher?.SelectedItems.Add(selectedItem);
                }
            }
            await RefreshModelInformation(currentItem);
        }

        /// <summary>
        /// Used to refresh all locators save for the <see cref="ModelSearcher" />.
        /// </summary>
        /// <param name="token">The token for use in locking.</param>
        /// <returns>Task for async use.</returns>
        protected Task RefreshDependentLocators(object token) =>
            Task.WhenAll(DependentLocators.Select(loc => loc.RefreshLocator(Model.Id, token)).ToArray());

        /// <summary>
        /// Used to clear, and disable all locators save for the <see cref="ModelSearcher" />.
        /// </summary>
        /// <param name="token"></param>
        protected void ClearAndDisableDependentLocators(object token) {
            foreach (var locator in DependentLocators)
                locator.ClearAndDisableLocator(token);
        }

        /// <summary>
        /// Logic used when the State changes.
        /// </summary>
        /// <param name="oldValue">The old State value.</param>
        protected override void OnStateChanged(ViewModelState oldValue) {
            base.OnStateChanged(oldValue);
            Navigation.State = State;
            switch (State) {
                case ViewModelState.Idle:
                    CriticalOperationType = CriticalOperationType.None;
                    ModelSearcher?.RequestEnable(_adminToken);
                    break;
                case ViewModelState.FamilialOperation:
                case ViewModelState.Restricted:
                    ModelSearcher?.RequestEnable(_adminToken);
                    break;
                case ViewModelState.NonCriticalOperation:
                case ViewModelState.CriticalOperation:
                    ModelSearcher?.RequestDisable(_adminToken);
                    break;
            }
        }

        /// <summary>
        /// Logic used when the CriticalOperationType changes.
        /// </summary>
        /// <param name="oldValue">The old CriticalOperationType value.</param>
        protected virtual void OnCriticalOperationTypeChanged(CriticalOperationType oldValue) {
            Navigation.CriticalOperationType = CriticalOperationType;
            switch (CriticalOperationType) {
                case CriticalOperationType.None:
                    Modifying = false;
                    RequestCriticalUnlock(_adminToken);
                    break;
                case CriticalOperationType.AddingNewRecord:
                case CriticalOperationType.ModifyingRecord:
                    Modifying = true;
                    RequestCriticalLock(_adminToken);
                    break;
            }
        }

        /// <summary>
        /// The logic used when a record has been requested. Only calls OnLocatorRecordRequested if it not already processing it for that particular locator.
        /// </summary>
        /// <param name="sender">The initiator of the event. Is used as token.</param>
        /// <param name="e">The EventArgs of the event.</param>
        private async void Locator_RecordRequested(object sender, EventArgs e) {
            if (locatorsRecordRequested.Contains(sender) == false) {
                locatorsRecordRequested.Add(sender);
                await OnLocatorRecordRequested(sender);
                locatorsRecordRequested.Remove(sender);
            }
        }

        /// <summary>
        /// Method that is run when Locator has signaled that a record is requested.
        /// </summary>
        /// <param name="sender">The locator that has a record that is requested.</param>
        /// <returns>Task for async use.</returns>
        protected virtual Task OnLocatorRecordRequested(object sender) => Task.CompletedTask;

        /// <summary>
        /// The logic used when a search has been completed.
        /// </summary>
        /// <param name="sender">The initiator of the event. Is used as token.</param>
        /// <param name="e">The EventArgs of the event.</param>
        private void Locator_SearchComplete(object sender, EventArgs e) {
            ModelSearcher?.RequestEnable(sender);
            RecordChangeLocker?.RequestUnlock(sender);
        }

        /// <summary>
        /// The logic used when a search has been initiated.
        /// </summary>
        /// <param name="sender">The initiator of the event. Is used as token.</param>
        /// <param name="e">The EventArgs of the event.</param>
        private void Locator_SearchToBegin(object sender, EventArgs e) {
            ModelSearcher?.RequestDisable(sender);
            RecordChangeLocker?.RequestLock(sender);
        }

        /// <summary>
        /// The logic used to update the <see cref="ModelSearcher" />.
        /// </summary>
        /// <param name="searcher">The new <see cref="ModelSearcher" />.</param>
        /// <returns>Task for async use.</returns>
        public async Task UpdateModelSearcher(IVMLocator<TModelSearcher> searcher) {
            var oldValue = _modelSearcher;
            if (Perform.ReplaceIfDifferent(ref _modelSearcher, searcher).WasSuccessful) {
                if (oldValue != null) {
                    Locators.Remove(oldValue);
                    oldValue.SelectedItems.CollectionChanged -= SelectedItems_CollectionChanged;
                    oldValue.ExportId -= ModelSearcher_ExportId;
                    oldValue.Cleared -= ModelSearcher_Cleared;
                }
                if (_modelSearcher != null) {
                    _modelSearcher.SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;
                    _modelSearcher.ExportId += ModelSearcher_ExportId;
                    _modelSearcher.Cleared += ModelSearcher_Cleared;
                    Locators.Add(_modelSearcher);
                    if (_modelSearcher.CurrentItems.Any()) {

                        var navCollection =
                            (_modelSearcher.SelectedItems.Take(2).Count() == 2 ?
                                _modelSearcher.SelectedItems :
                                _modelSearcher.CurrentItems
                            );

                        _backForwardNavigator.UpdateNavigationCollection(navCollection.Select(item => item.Id));

                        int currentItem =
                            (_modelSearcher.SelectedItems.Any() ?
                                _modelSearcher.SelectedItems :
                                _modelSearcher.CurrentItems
                            ).First().Id;
                        await _backForwardNavigator.UpdateCurrentItem(currentItem);
                    }
                }
                await OnModelSearcherSwapped(oldValue);
                OnPropertyChanged(nameof(ModelSearcher));
            }
        }

        protected override EventArgs GetSuccessObject() => EventArgs.Empty;

        /// <summary>
        /// Used to dispose of resources in use by the <see cref="AdminViewModel{TViewModel, TModel, TModelSearcher}"/>
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing) {
            if (!_disposedValue) {
                if (disposing) {
                    ApplicationRestrictor.RequestUnlock(_adminToken);
                    FamilialLocker.RequestUnlock(_adminToken);
                    CriticalOperationType = CriticalOperationType.None;
                    if (ModelSearcher != null) {
                        ModelSearcher.SelectedItems.CollectionChanged -= SelectedItems_CollectionChanged;
                        ModelSearcher.ExportId -= ModelSearcher_ExportId;
                        ModelSearcher.Cleared -= ModelSearcher_Cleared;
                        ModelSearcher.RequestEnable(_adminToken);
                    }
                    Model.ErrorsChanged -= Model_ErrorsChanged;
                    Model.Dispose();
                    _newEnableCallback = null;
                    _duplicateEnableCallback = null;
                    _modifyEnableCallback = null;
                    _backEnableCallback = null;
                    _forwardEnableCallback = null;
                    _performDeleteOnRecordEnableCallback = null;
                    _backForwardNavigator.CurrentItemChangedCallback = null;
                    _backForwardNavigator?.Dispose();
                    Navigation.Dispose();
                    RecordChangeLocker.Dispose();
                    ConfirmCancelLocker.Dispose();
                    NewRecordCancelLocker.Dispose();
                    NavigationalOverride.Dispose();
                    OutsideModificationLocker.Dispose();
                    DefaultModelCallback = null;
                    Locators.ForEach(locator => {
                        locator.SearchToBegin -= Locator_SearchToBegin;
                        locator.SearchComplete -= Locator_SearchComplete;
                        locator.RecordRequested -= Locator_RecordRequested;
                        locator.RequestEnable(_adminToken);
                    });
                    Locators.Clear();
                }
                _disposedValue = true;
            }
            base.Dispose(disposing);
        }

    }
}
