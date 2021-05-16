namespace CSharpToolkit.DataAccess {
    using Utilities;
    using System;
    using System.ComponentModel;
    public class PropertyItem : INotifyPropertyChanged {

        object _original;
        object _current;

        public event PropertyChangedEventHandler PropertyChanged;

        public PropertyItem(string name) {
            PropertyName = name;
        }

        public string PropertyName { get; }
        public Type PropertyType { get; set; }
        public object Current {
            get { return _current; }
            set {
                if (Perform.ReplaceIfDifferent(ref _current, value).WasSuccessful) {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
                }
            }
        }
        public object Original { get { return _original; } set { _original = value; } }

        public bool HasPropertyChanged {
            get {
                if (Current == null) {
                    if (Original == null)
                        return false;
                    else
                        return true;
                }
                return (Current).Equals((Original)) == false;
            }
        }

        public void Save() =>
            Original = Current;
        public void Reset() =>
            Current = Original;

        public PropertyItem Clone() =>
            new PropertyItem(PropertyName) {
                Current = Current,
                Original = Current,
                PropertyType = PropertyType,
            };

    }
}
