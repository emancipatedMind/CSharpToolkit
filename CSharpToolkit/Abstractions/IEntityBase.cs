namespace CSharpToolkit.Abstractions {
    using System.ComponentModel;
    public interface IEntityBase : IUserNotifier, INotifyDataErrorInfo, INotifyPropertyChanged, IDataErrorInfo {
    }
}