namespace CSharpToolkit.XAML {
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    public class ListViewItemWithSlide : ListViewItem {

        public ListViewItemWithSlide() : base() { }

        public Brush SlideBrush {
            get { return (Brush)GetValue(SlideBrushProperty); }
            set { SetValue(SlideBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SlideBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SlideBrushProperty =
            DependencyProperty.Register("SlideBrush", typeof(Brush), typeof(ListViewItemWithSlide), new UIPropertyMetadata(Brushes.Transparent));
    }
}