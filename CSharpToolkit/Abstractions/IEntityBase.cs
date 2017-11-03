namespace CSharpToolkit.Abstractions {
    using System.ComponentModel;
    public interface IEntityBase : IStringNotification, INotifyDataErrorInfo, INotifyPropertyChanged, IDataErrorInfo {
    }
}