namespace CSharpToolkit.XAML.Behaviors {
    using System.Reflection;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Controls;
    public class DataGridBehaviors {

        public static void SetDisallowRowFocus(DependencyObject obj, bool value) {
            obj.SetValue(DisallowRowFocusProperty, value);
        }

        // Using a DependencyProperty as the backing store for DisallowRowFocus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisallowRowFocusProperty =
            DependencyProperty.RegisterAttached("DisallowRowFocus", typeof(bool), typeof(DataGridBehaviors), new PropertyMetadata(false, DataGrid_DisallowRowFocusChanged));

        private static void DataGrid_DisallowRowFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is DataGrid == false)
                return;

            var dataGrid = (DataGrid)d;

            if ((bool)e.NewValue)
                dataGrid.SelectionChanged += DataGrid_SelectionChanged;
            else
                dataGrid.SelectionChanged -= DataGrid_SelectionChanged;
        }

        private static void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
            (sender as UIElement)?.Focus();

        public static void SetApplySortFix(DependencyObject obj, bool value) {
            obj.SetValue(ApplySortFixProperty, value);
        }

        // Using a DependencyProperty as the backing store for ApplySortFix.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ApplySortFixProperty =
            DependencyProperty.RegisterAttached("ApplySortFix", typeof(bool), typeof(DataGridBehaviors), new PropertyMetadata(false, ApplySortFixPropertyChanged));

        private static void ApplySortFixPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var dataGrid = d as DataGrid;
            if (dataGrid == null)
                return;

            if ((bool)e.NewValue) {
                dataGrid.Sorting += DataGrid_Sorting;
            }
            else {
                dataGrid.Sorting -= DataGrid_Sorting;
            }
        }

        private static void DataGrid_Sorting(object sender, DataGridSortingEventArgs e) {
            var dataGrid = (DataGrid)sender;

            IList itemsSource = dataGrid.ItemsSource as IList;
            if (itemsSource == null)
                return;

            DataGridColumn sortColumn = dataGrid.Columns.FirstOrDefault(c => c.Equals(e.Column));
            if (sortColumn == null)
                return;

            Type columnType = sortColumn.GetType();
            PropertyInfo bindingProp = columnType.GetProperty("Binding");
            if (bindingProp == null)
                return;

            var binding = bindingProp.GetValue(sortColumn) as Binding;
            if (binding == null)
                return;

            string sortParameter = binding.Path.Path;

            Type objectType = dataGrid.ItemsSource.GetType().GetGenericArguments().Single();
            PropertyInfo sortProperty = objectType.GetProperty(sortParameter);
            if (sortProperty == null)
                return;

            DataGridEnvironment environment = GetDataGridEnvironment(dataGrid);
            if (environment == null && itemsSource is INotifyCollectionChanged) {
                environment = new DataGridEnvironment(dataGrid);
                var notifyCollectionChanged = (INotifyCollectionChanged)itemsSource;
                notifyCollectionChanged.CollectionChanged += environment.CollectionChanged_CollectionChanged;
                SetDataGridEnvironment(dataGrid, environment);
            }
            environment?.DisableReset(true);

            Func<object, object> sortFunction = obj => sortProperty.GetValue(obj);

            IEnumerable<object> sortedSelectedItems;
            IEnumerable<object> sortedAllItems;

            if (sortColumn.SortDirection == System.ComponentModel.ListSortDirection.Ascending) {
                sortedSelectedItems = dataGrid.SelectedItems.Cast<object>().OrderByDescending(sortFunction).ToList();
                sortedAllItems = dataGrid.Items.Cast<object>().OrderByDescending(sortFunction).ToList();
                sortColumn.SortDirection = System.ComponentModel.ListSortDirection.Descending;
            }
            else {
                sortedSelectedItems = dataGrid.SelectedItems.Cast<object>().OrderBy(sortFunction).ToList();
                sortedAllItems = dataGrid.Items.Cast<object>().OrderBy(sortFunction).ToList();
                sortColumn.SortDirection = System.ComponentModel.ListSortDirection.Ascending;
            }

            itemsSource.Clear();
            if (dataGrid.SelectionMode == DataGridSelectionMode.Extended)
                dataGrid.SelectedItems.Clear();
            else
                dataGrid.SelectedItem = null;

            foreach (var item in sortedAllItems)
                itemsSource.Add(item);

            if (dataGrid.SelectionMode == DataGridSelectionMode.Extended) {
                foreach (var item in sortedSelectedItems)
                    dataGrid.SelectedItems.Add(item);
            }
            else
                dataGrid.SelectedItem = sortedSelectedItems.FirstOrDefault();

            e.Handled = true;
            environment?.DisableReset(false);
        }

        static DataGridEnvironment GetDataGridEnvironment(DependencyObject obj) =>
            (DataGridEnvironment)obj.GetValue(DataGridEnvironmentProperty);

        static void SetDataGridEnvironment(DependencyObject obj, DataGridEnvironment value) =>
            obj.SetValue(DataGridEnvironmentProperty, value);

        // Using a DependencyProperty as the backing store for DataGridEnvironment.  This enables animation, styling, binding, etc...
        static readonly DependencyProperty DataGridEnvironmentProperty =
            DependencyProperty.RegisterAttached("DataGridEnvironment", typeof(DataGridEnvironment), typeof(DataGridBehaviors), new PropertyMetadata(null));

        class DataGridEnvironment {
            DataGrid _dataGrid;
            bool _disabled;

            public DataGridEnvironment(DataGrid dataGrid) {
                _dataGrid = dataGrid;
            }

            public void DisableReset(bool disable) {
                _disabled = disable;
            } 

            public void CollectionChanged_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
                if (_disabled)
                    return;

                IList list = (IList)sender; 
                if (list.Count == 0) {
                    foreach (var column in _dataGrid.Columns)
                        column.SortDirection = null;
                }
            }
        }

    }
}