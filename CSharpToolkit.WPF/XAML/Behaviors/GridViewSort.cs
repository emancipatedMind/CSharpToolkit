namespace CSharpToolkit.XAML.Behaviors {
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Documents;
    /*
        On ListView, place behavior SetAutoSort to true, and then on each column header, SetPropertyName to property to sort on. If that particular column cannot sort, just skip it.
    */
    public class GridViewSort {
        #region Public attached properties

        public static ICommand GetCommand(DependencyObject obj) =>
            (ICommand)obj.GetValue(CommandProperty);

        public static void SetCommand(DependencyObject obj, ICommand value) =>
            obj.SetValue(CommandProperty, value);

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(GridViewSort),
                new UIPropertyMetadata(
                    null,
                    (o, e) => {
                        ItemsControl listView = o as ItemsControl;
                        if (listView != null) {
                            if (!GetAutoSort(listView)) {
                                // Don't change click handler if AutoSort enabled
                                if (e.OldValue != null && e.NewValue == null) {
                                    listView.RemoveHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
                                }
                                if (e.OldValue == null && e.NewValue != null) {
                                    listView.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
                                }
                            }
                        }
                    }
                )
            );

        public static bool GetAutoSort(DependencyObject obj) =>
            (bool)obj.GetValue(AutoSortProperty);

        public static void SetAutoSort(DependencyObject obj, bool value) =>
            obj.SetValue(AutoSortProperty, value);

        // Using a DependencyProperty as the backing store for AutoSort.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoSortProperty =
            DependencyProperty.RegisterAttached(
                "AutoSort",
                typeof(bool),
                typeof(GridViewSort),
                new UIPropertyMetadata(
                    false,
                    (o, e) => {
                        if (o is ListView) {
                            ListView listView = (ListView)o;
                            bool oldValue = (bool)e.OldValue;
                            bool newValue = (bool)e.NewValue;

                            if (GetCommand(listView) != null || oldValue == newValue)
                                return;

                            if (newValue)
                                listView.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
                            else
                                listView.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
                        }
                    }
                ));

        public static string GetPropertyName(DependencyObject obj) =>
            (string)obj.GetValue(PropertyNameProperty);

        public static void SetPropertyName(DependencyObject obj, string value) =>
            obj.SetValue(PropertyNameProperty, value);

        // Using a DependencyProperty as the backing store for PropertyName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.RegisterAttached(
                "PropertyName",
                typeof(string),
                typeof(GridViewSort),
                new UIPropertyMetadata(null)
            );

        public static bool GetShowSortGlyph(DependencyObject obj) =>
            (bool)obj.GetValue(ShowSortGlyphProperty);

        public static void SetShowSortGlyph(DependencyObject obj, bool value) =>
            obj.SetValue(ShowSortGlyphProperty, value);

        // Using a DependencyProperty as the backing store for ShowSortGlyph.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowSortGlyphProperty =
            DependencyProperty.RegisterAttached("ShowSortGlyph", typeof(bool), typeof(GridViewSort), new UIPropertyMetadata(true));

        public static ImageSource GetSortGlyphAscending(DependencyObject obj) =>
            (ImageSource)obj.GetValue(SortGlyphAscendingProperty);

        public static void SetSortGlyphAscending(DependencyObject obj, ImageSource value) =>
            obj.SetValue(SortGlyphAscendingProperty, value);

        // Using a DependencyProperty as the backing store for SortGlyphAscending.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SortGlyphAscendingProperty =
            DependencyProperty.RegisterAttached("SortGlyphAscending", typeof(ImageSource), typeof(GridViewSort), new UIPropertyMetadata(null));

        public static ImageSource GetSortGlyphDescending(DependencyObject obj) =>
            (ImageSource)obj.GetValue(SortGlyphDescendingProperty);

        public static void SetSortGlyphDescending(DependencyObject obj, ImageSource value) =>
            obj.SetValue(SortGlyphDescendingProperty, value);

        // Using a DependencyProperty as the backing store for SortGlyphDescending.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SortGlyphDescendingProperty =
            DependencyProperty.RegisterAttached("SortGlyphDescending", typeof(ImageSource), typeof(GridViewSort), new UIPropertyMetadata(null));

        #endregion

        #region Private attached properties

        private static GridViewColumnHeader GetSortedColumnHeader(DependencyObject obj) =>
            (GridViewColumnHeader)obj.GetValue(SortedColumnHeaderProperty);

        private static void SetSortedColumnHeader(DependencyObject obj, GridViewColumnHeader value) =>
            obj.SetValue(SortedColumnHeaderProperty, value);

        // Using a DependencyProperty as the backing store for SortedColumn.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty SortedColumnHeaderProperty =
            DependencyProperty.RegisterAttached("SortedColumnHeader", typeof(GridViewColumnHeader), typeof(GridViewSort), new UIPropertyMetadata(null));
        #endregion

        #region Column header click event handler

        private static void ColumnHeader_Click(object sender, RoutedEventArgs e) {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;

            if (headerClicked == null || headerClicked.Column == null)
                return;

            string propertyName = GetPropertyName(headerClicked.Column);
            if (string.IsNullOrEmpty(propertyName))
                return;

            ListView listView = GetAncestor<ListView>(headerClicked);
            if (listView == null)
                return;

            ICommand command = GetCommand(listView);
            if (command != null) {
                if (command.CanExecute(propertyName))
                    command.Execute(propertyName);
            }
            else if (GetAutoSort(listView)) {
                ApplySort(listView.Items, propertyName, listView, headerClicked);
            }

        }
        #endregion

        #region Helper methods
        public static T GetAncestor<T>(DependencyObject reference) where T : DependencyObject {
            DependencyObject parent = VisualTreeHelper.GetParent(reference);

            while (parent != null && parent is T == false)
                parent = VisualTreeHelper.GetParent(parent);

            return parent as T;
        }

        public static void ApplySort(ICollectionView view, string propertyName, ListView listView, GridViewColumnHeader sortedColumnHeader) {
            ListSortDirection direction = ListSortDirection.Ascending;

            if (view.SortDescriptions.Any()) {
                SortDescription currentSort = view.SortDescriptions[0];
                if (currentSort.PropertyName == propertyName) {
                    if (currentSort.Direction == ListSortDirection.Ascending)
                        direction = ListSortDirection.Descending;
                    else
                        direction = ListSortDirection.Ascending;
                }
                view.SortDescriptions.Clear();

                GridViewColumnHeader currentSortedColumnHeader = GetSortedColumnHeader(listView);
                RemoveSortGlyph(currentSortedColumnHeader);
            }

            if (string.IsNullOrEmpty(propertyName)) return;

            view.SortDescriptions.Add(new SortDescription(propertyName, direction));

            if (GetShowSortGlyph(listView))
                AddSortGlyph(
                    sortedColumnHeader,
                    direction,
                    direction == ListSortDirection.Ascending ?
                        GetSortGlyphAscending(listView) :
                        GetSortGlyphDescending(listView));

            SetSortedColumnHeader(listView, sortedColumnHeader);
        }

        private static void AddSortGlyph(GridViewColumnHeader columnHeader, ListSortDirection direction, ImageSource sortGlyph) {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(columnHeader);
            adornerLayer.Add(new SortGlyphAdorner(columnHeader, direction, sortGlyph));
        }

        private static void RemoveSortGlyph(GridViewColumnHeader columnHeader) {
            if (columnHeader == null)
                return;

            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(columnHeader);
            Adorner[] adorners = adornerLayer.GetAdorners(columnHeader);
            if (adorners == null) return;

            foreach (Adorner adorner in adorners) {
                if (adorner is SortGlyphAdorner)
                    adornerLayer.Remove(adorner);
            }
        }
        #endregion
    }
}