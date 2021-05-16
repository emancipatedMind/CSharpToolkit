namespace CSharpToolkit.ViewModels.Abstractions {
    using System.Threading.Tasks;
    using Utilities;
    using Utilities.Abstractions;

    public abstract class SingleModelAdminViewModel<TViewModel, TModel, TModelSearcher> : TemplatedAdminViewModel<TViewModel, TModel, TModel, TModelSearcher> where TViewModel : EditableBase<TModel>, TModel, new() {
        public SingleModelAdminViewModel(ISimpleInteractor<TModel, TModel> interactor) : base(interactor) { }

        protected override Task ConfirmNoModify() => Task.CompletedTask;
        protected override Task ConfirmUpdateSuccessful(OperationResult result) => Task.CompletedTask;
        protected override Task CreateToRun() => Task.CompletedTask;

        protected override Task RefreshSuccessful(TModel result) {
            Model.Load(result);
            return Task.CompletedTask;
        }
    }
}
