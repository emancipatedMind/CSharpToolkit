namespace CSharpToolkit.ViewModels {
    using CSharpToolkit.XAML;
    public class IsSelectedViewModel : EntityBase {
        bool _isSelected = false;
        public bool IsSelected {
            get { return _isSelected; }
            set { FirePropertyChangedIfDifferent(ref _isSelected, value); }
        }

    }
}
