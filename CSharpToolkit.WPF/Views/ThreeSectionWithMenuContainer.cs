namespace CSharpToolkit.Views {
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    public class ThreeSectionWithMenuContainer : TwoSectionWithMenuContainer {
        static ThreeSectionWithMenuContainer() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ThreeSectionWithMenuContainer), new FrameworkPropertyMetadata(typeof(ThreeSectionWithMenuContainer)));
        }

        public object SectionThree { get; set; }

    }
}
