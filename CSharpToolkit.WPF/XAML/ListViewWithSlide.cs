namespace CSharpToolkit.XAML {
    using System.Windows;
    using System.Windows.Controls;
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(ListViewItemWithSlide))]
    public partial class ListViewWithSlide : ListView {
        protected override DependencyObject GetContainerForItemOverride() =>
            new ListViewItemWithSlide();
        protected override bool IsItemItsOwnContainerOverride(object item) =>
            (item is ListViewItemWithSlide);

    }
}