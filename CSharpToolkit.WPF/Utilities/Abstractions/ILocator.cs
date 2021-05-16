namespace CSharpToolkit.Utilities.Abstractions {
    using CSharpToolkit.Utilities.Abstractions;
    using CSharpToolkit.Utilities.EventArgs;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    public interface ILocator {
        IList<IIdProvider> CurrentItems { get; }
        ObservableCollection<IIdProvider> SelectedItems { get; }

        event EventHandler Cleared;
        event EventHandler<GenericEventArgs<int>> ExportId;
        event EventHandler RecordRequested;

        int Constraint { get; set; }
        Task RefreshCollections();
        void RequestClear();
        void RequestDisable(object token);
        void RequestEnable(object token);
        void ClearSelectedItem();
        Task RequestSearch();
        void Remove(IIdProvider item);

        event EventHandler SearchToBegin;
        event EventHandler<GenericEventArgs<OperationResult>> SearchComplete;
    }
}