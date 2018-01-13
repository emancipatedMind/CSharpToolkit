namespace Dispatch.App_Data.Resources {
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    public partial class DoubleClickListViewItem : ListViewItem {

        public DoubleClickListViewItem() : base() { }

        public ICommand DoubleClickCommand {
            get { return (ICommand)GetValue(DoubleClickCommandProperty); }
            set { SetValue(DoubleClickCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DoubleClickCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DoubleClickCommandProperty =
            DependencyProperty.Register("DoubleClickCommand", typeof(ICommand), typeof(DoubleClickListViewItem), new UIPropertyMetadata());

    }
}