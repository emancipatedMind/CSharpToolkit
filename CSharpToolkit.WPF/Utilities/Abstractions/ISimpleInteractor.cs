namespace CSharpToolkit.Utilities.Abstractions {
    using System;
    using System.ComponentModel;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CSharpToolkit.Utilities;
    using CSharpToolkit.DataAccess;
    using CSharpToolkit.DataAccess.Abstractions;

    public interface ISimpleInteractor<TDataOrder, TReturnType> : ISimpleAdminDataAccessor<TDataOrder, TReturnType>, IFormInteractor, IOwnerProvider, System.IDisposable {
        Task<OperationResult<BeginNewResult<TReturnType>>> BeginDuplicateAsync(TDataOrder model);
        Task<OperationResult<BeginNewResult<TReturnType>>> BeginNewAsync(TDataOrder model);
        Task<OperationResult> BeginModificationsAsync(TDataOrder model);
        ModificationMode ModificationMode { get; }
        Task<OperationResult<CascadingDeleteType>> CascadingDeleteAsync(TDataOrder model);
        Task<OperationResult> CompleteCancel(TDataOrder model);
        Task<OperationResult<ConfirmRecordResult>> CompleteConfirmation(TDataOrder model);
        Task Model_ValidationRequested(TDataOrder model, string propName, List<Tuple<string, OperationResult>> validationList);
        Task<OperationResult<System.Collections.Generic.List<CSharpToolkit.DataAccess.PropertyModification>>> PropertyChanged(TDataOrder model, System.ComponentModel.PropertyChangedEventArgs e);
    }
}
