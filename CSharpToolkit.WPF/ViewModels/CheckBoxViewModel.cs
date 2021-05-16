namespace CSharpToolkit.ViewModels {
    using CSharpToolkit.XAML;
    public class CheckBoxViewModel : EntityBase {

        string _description;
        bool _isChecked;
        bool _enabled;
        string _value;

        public string Description {
            get { return _description; }
            set { FirePropertyChangedIfDifferent(ref _description, value); }
        }

        public bool IsChecked {
            get { return _isChecked; }
            set { FirePropertyChangedIfDifferent(ref _isChecked, value); }
        }

        public string Value {
            get { return _value; }
            set { FirePropertyChangedIfDifferent(ref _value, value); }
        }

        public bool Enabled {
            get { return _enabled; }
            set { FirePropertyChangedIfDifferent(ref _enabled, value); }
        }

    }
}
