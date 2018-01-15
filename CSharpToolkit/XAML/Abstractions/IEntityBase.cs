namespace CSharpToolkit.XAML.Abstractions {
    using Utilities.Abstractions;
    using System.ComponentModel;
    public interface IEntityBase : IUserNotifier, INotifyDataErrorInfo, INotifyPropertyChanged, IDataErrorInfo {
    }
}