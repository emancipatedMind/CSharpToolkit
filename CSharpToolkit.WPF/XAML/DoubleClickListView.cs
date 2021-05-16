namespace CSharpToolkit.XAML {
    using System.Windows;
    using System.Windows.Controls;
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(DoubleClickListViewItem))]
    public class DoubleClickListView : ListView {
        protected override DependencyObject GetContainerForItemOverride() {
            return new DoubleClickListViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item) {
            return (item is DoubleClickListViewItem);
        }
    }
}