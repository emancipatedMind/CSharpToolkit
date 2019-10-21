namespace CSharpToolkit.XAML.Behaviors {
    using System.Windows;
    using System.Windows.Controls;

    public class TreeViewHelper {

        public static object GetSelectedItem(DependencyObject obj) => obj.GetValue(SelectedItemProperty);
        public static void SetSelectedItem(DependencyObject obj, object value) => obj.SetValue(SelectedItemProperty, value);

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.RegisterAttached("SelectedItem", typeof(object), typeof(TreeViewHelper), new UIPropertyMetadata(new object(), SelectedItemChanged));

        private static void SelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
            if (obj is TreeView == false)
                return;

            var treeView = (TreeView)obj;
            TreeViewSelectedItemBehavior view = GetTreeViewSelectedItemBehavior(treeView);

            if (view == null) {
                view = new TreeViewSelectedItemBehavior(treeView);
                SetTreeViewSelectedItemBehavior(obj, view);
            }
        }

        static TreeViewSelectedItemBehavior GetTreeViewSelectedItemBehavior(DependencyObject obj) => (TreeViewSelectedItemBehavior)obj.GetValue(TreeViewSelectedItemBehaviorProperty);
        static void SetTreeViewSelectedItemBehavior(DependencyObject obj, TreeViewSelectedItemBehavior value) => obj.SetValue(TreeViewSelectedItemBehaviorProperty, value);

        // Using a DependencyProperty as the backing store for TreeViewSelectedItemBehavior.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TreeViewSelectedItemBehaviorProperty =
            DependencyProperty.RegisterAttached("TreeViewSelectedItemBehavior", typeof(TreeViewSelectedItemBehavior), typeof(TreeViewHelper), new PropertyMetadata(null));

        private class TreeViewSelectedItemBehavior {

            TreeView view;
            public TreeViewSelectedItemBehavior(TreeView view) {
                this.view = view;
                view.SelectedItemChanged += (sender, e) => SetSelectedItem(view, e.NewValue);
            }
        }

    }
}
