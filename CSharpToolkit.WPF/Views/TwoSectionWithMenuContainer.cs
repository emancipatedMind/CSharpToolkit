namespace CSharpToolkit.Views {
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    public class TwoSectionWithMenuContainer : Control {

        static TwoSectionWithMenuContainer() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TwoSectionWithMenuContainer), new FrameworkPropertyMetadata(typeof(TwoSectionWithMenuContainer)));
        }

        public object SectionOne { get; set; }
        public object SectionTwo { get; set; }
        public object ContainerMenu { get; set; }

        public Brush ContainerBrush {
            get { return (Brush)GetValue(ContainerBrushProperty); }
            set { SetValue(ContainerBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContainerBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContainerBrushProperty =
            DependencyProperty.Register(nameof(ContainerBrush), typeof(Brush), typeof(TwoSectionWithMenuContainer), new PropertyMetadata(null));

        public GridLength SectionOneWidth {
            get { return (GridLength)GetValue(SectionOneWidthProperty); }
            set { SetValue(SectionOneWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SectionOneWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SectionOneWidthProperty =
            DependencyProperty.Register(nameof(SectionOneWidth), typeof(GridLength), typeof(TwoSectionWithMenuContainer), new PropertyMetadata(new GridLength(1, GridUnitType.Auto)));

        public GridLength SectionTwoWidth {
            get { return (GridLength)GetValue(SectionTwoWidthProperty); }
            set { SetValue(SectionTwoWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SectionTwoWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SectionTwoWidthProperty =
            DependencyProperty.Register(nameof(SectionTwoWidth), typeof(GridLength), typeof(TwoSectionWithMenuContainer), new PropertyMetadata(new GridLength(1, GridUnitType.Auto)));

    }
}
