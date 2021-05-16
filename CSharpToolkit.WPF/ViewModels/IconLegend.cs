namespace CSharpToolkit.ViewModels {
    using CSharpToolkit.XAML;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public class IconLegend : EntityBase {

        string _text;
        BitmapImage _icon;

        public IconLegend() { }
        public IconLegend(string text, BitmapImage icon) {
            _text = text;
            _icon = icon;
        }

        public string Text {
            get { return _text; }
            set { FirePropertyChangedIfDifferent(ref _text, value); }
        }

        public BitmapImage Icon {
            get { return _icon; }
            set { FirePropertyChangedIfDifferent(ref _icon, value); }
        }

    }
}
