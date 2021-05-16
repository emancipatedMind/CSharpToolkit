namespace CSharpToolkit.ViewModels.Abstractions {
    using CSharpToolkit.Utilities;
    using CSharpToolkit.Extensions;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using CSharpToolkit.Utilities.Abstractions;
    using CSharpToolkit.DataAccess;
    using CSharpToolkit.ViewModels.Abstractions;

    public abstract class TemplatedAdminViewModel<TViewModel, TModel, TDataOrder, TModelSearcher> : AdminViewModel<TViewModel, TModel, TModelSearcher> where TViewModel : EditableBase<TModel>, TModel, new() {

        readonly static System.Reflection.PropertyInfo[] ModelProperties = (
            from mProp in typeof(TModel).GetProperties()
            join vProp in typeof(TViewModel).GetProperties() on mProp.Name equals vProp.Name
            select vProp
            ).ToArray();

        bool _disposed;
        ISimpleInteractor<TModel, TDataOrder> _interactor;
        Locker _validationLoadLocker = new Locker();

        public TemplatedAdminViewModel(ISimpleInteractor<TModel, TDataOrder> interactor) {
            _interactor = interactor;
            Model.RunValidationCallback = Model_ValidationRequested;
            Model.PropertyChanged += Model_PropertyChanged;
            PropertyLock.LockStatusChanged += PropertyLock_LockStatusChanged;
        }

        protected ILocker PropertyLock { get; } = new Locker();

        // Template Methods
        protected abstract Task RefreshSuccessful(TDataOrder result);
        protected abstract Task CreateToRun();
        protected abstract Task ConfirmNoModify();
        protected abstract Task ConfirmUpdateSuccessful(OperationResult result);
        // End Template Methods

        public sealed override Task RefreshModelInformation(int id) =>
            RefreshModelInformation(id, true);

        public virtual async Task RefreshModelInformation(int id, bool totalRefresh) {
            if (id <= 0)
                return;
            Model.Id = id;

            OperationResult<TDataOrder> operation = await
                RunOperationThroughNonCriticalState(() => _interactor.LookupAsync(Model));

            if (operation.WasSuccessful) {
                await RefreshSuccessful(operation.Result);
                if (totalRefresh)
                    await RefreshDependentLocators(_adminToken);
            }
            else {
                await BackForwardNavigator.UpdateCurrentItem(-1);
                Model.Clear();
                ClearAndDisableDependentLocators(_adminToken);
            }
        }

        protected async override Task ModifyCallback() {
            CriticalOperationType = CriticalOperationType.ModifyingRecord;

            var operation = await _interactor.BeginModificationsAsync(Model);
            if (operation.WasSuccessful == false) {
                await CancelCallback();
            }
        }

        protected override Task DuplicateCallback() =>
            Create(() => _interactor.BeginDuplicateAsync(Model));
        protected override Task NewCallback() =>
            Create(() => {
                PropertyLock.LockForDuration(Model.Clear);
                return _interactor.BeginNewAsync(DefaultModel);
            });

        private Task Create(Func<Task<OperationResult<BeginNewResult<TDataOrder>>>> callback) =>
            OutsideModificationLocker.LockForDurationAsync(async () => {
                CriticalOperationType = CriticalOperationType.AddingNewRecord;

                await CreateToRun();

                OperationResult<BeginNewResult<TDataOrder>> operation =
                    await callback();

                if (operation.WasSuccessful) {
                    if (operation.Result.Type == BeginNewType.Refresh)
                        await RefreshModelInformation(operation.Result.Id, true);
                    else if (operation.Result.Type == BeginNewType.Load)
                        await RefreshSuccessful(operation.Result.Model);
                }
                else {
                    await CancelCallback();
                }
            });

        protected override Task CancelCallback() =>
            OutsideModificationLocker.LockForDurationAsync(async () => {
                int oldId = Model.Id;

                await _interactor.CompleteCancel(Model);

                if (CriticalOperationType == CriticalOperationType.ModifyingRecord) {
                    bool totalRefreshRequired = oldId != Model.Id;
                    await RefreshModelInformation(Model.Id, totalRefreshRequired);
                }
                else
                    OnModelSearcherCleared();

                CriticalOperationType = CriticalOperationType.None;
            });

        protected override Task ConfirmCallback() =>
            OutsideModificationLocker.LockForDurationAsync(async () => {
                bool modifying = CriticalOperationType == CriticalOperationType.ModifyingRecord;

                OperationResult<ConfirmRecordResult> operation =
                    await _interactor.CompleteConfirmation(Model);

                if (operation.WasSuccessful) {
                    CriticalOperationType = CriticalOperationType.None;

                    if (operation.Result.Id > 0) {
                        await RefreshModelInformation(operation.Result.Id, true);
                    }
                    else if (operation.Result.Types.Any(t => t == ConfirmRecordType.DoNotRefresh) == false) {
                        await RefreshModelInformation(Model.Id, modifying == false);
                    }

                    if (operation.Result.Types.Any(t => t == ConfirmRecordType.RefreshCollections)) {
                        await RefreshCollections();
                        foreach (var locator in Locators)
                            await locator.RefreshCollections();
                    }
                }
            });

        protected override Task OpenMainWindowCallback() =>
            _interactor.ActivateMainWindow();

        protected override async Task PerformDeleteOnRecordCallback() {
            int id = Model.Id;
            var operation = await _interactor.CascadingDeleteAsync(Model);
            if (operation.WasSuccessful == false) {
                return;
            }
            switch (operation.Result) {
                case CascadingDeleteType.Edit:
                    await RefreshModelInformation(Model.Id, false);
                    break;
                case CascadingDeleteType.Delete:
                    OnModelSearcherCleared();
                    if (ModelSearcher != null) {
                        CSharpToolkit.Utilities.Abstractions.IIdProvider item = ModelSearcher.CurrentItems.FirstOrDefault(m => m.Id == id);
                        if (item != null)
                            ModelSearcher.Remove(item);
                    }
                    break;
            }
        }

        protected override Task PrintFormCallback() =>
            Task.CompletedTask;

        private void PropertyLock_LockStatusChanged(object sender, CSharpToolkit.Utilities.EventArgs.GenericEventArgs<LockStatus> e) {
            if (PropertyLock.IsFree()) {
                Model.PropertyChanged += Model_PropertyChanged;
            }
            else {
                Model.PropertyChanged -= Model_PropertyChanged;
            }
        }

        private async void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (Modifying == false) {
                return;
            }

            bool isModelProperty = ModelProperties.Select(prop => prop.Name).Any(name => name == e.PropertyName);
            if (isModelProperty == false) {
                return;
            }

            await new[] { ConfirmCancelLocker, PropertyLock }.LockForDurationAsync(async () => {
                var operation = await _interactor.PropertyChanged(Model, e);
                if (operation.WasSuccessful == false)
                    return;
                ModelLoadFromPropertyModification(operation.Result);
            });
        }

        protected void ModelLoadFromPropertyModification(IEnumerable<PropertyModification> properties) {
            if ((properties?.Any() ?? false) == false)
                return;

            new[] { ConfirmCancelLocker, PropertyLock }.LockForDuration(() => {
                var validChangedProperties = (
                    from prop in ModelProperties
                    join cP in properties on prop.Name equals cP.PropertyName
                    select new { PropertyInfo = prop, NewValue = cP.NewValue }
                ).ToArray();

                foreach (var x in validChangedProperties) {
                    try {
                        x.PropertyInfo.SetValue(Model, x.NewValue);
                    }
                    catch { }
                }
            });
        } 

        private Task Model_ValidationRequested(string property, object value, List<Tuple<string, OperationResult>> validationResult) =>
            _interactor.Model_ValidationRequested(Model, property, validationResult);

        protected override void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    _validationLoadLocker.Dispose();
                    PropertyLock.Dispose();
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }
    }
}
