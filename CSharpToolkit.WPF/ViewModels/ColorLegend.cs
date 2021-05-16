namespace CSharpToolkit.ViewModels {
    using CSharpToolkit.XAML;
    using System.Windows.Media;

    public class ColorLegend : EntityBase {

        string _text;
        Brush _brush;

        public ColorLegend() { }
        public ColorLegend(string text, Brush brush) {
            _text = text;
            _brush = brush;
        }

        public string Text {
            get { return _text; }
            set { FirePropertyChangedIfDifferent(ref _text, value); }
        }

        public Brush Brush {
            get { return _brush; }
            set { FirePropertyChangedIfDifferent(ref _brush, value); }
        }

    }
}
