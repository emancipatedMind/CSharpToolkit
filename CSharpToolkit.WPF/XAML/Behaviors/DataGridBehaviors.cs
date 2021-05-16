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
    using CSharpToolkit.Extensions;
    using Utilities;
    using System.Windows.Input;
    using System.Windows.Media;
    using Abstractions;
    using System.Threading.Tasks;

    public class DataGridBehaviors {

        public static void SetDisallowRowFocus(DependencyObject obj, bool value) =>
            obj.SetValue(DisallowRowFocusProperty, value);

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

        public static Comparison<object> GetApplySortFixSorter(DependencyObject obj) =>
            (Comparison<object>)obj.GetValue(ApplySortFixSorterProperty);

        public static void SetApplySortFixSorter(DependencyObject obj, Comparison<object> value) =>
            obj.SetValue(ApplySortFixSorterProperty, value);

        // Using a DependencyProperty as the backing store for ApplySortFixSorter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ApplySortFixSorterProperty =
            DependencyProperty.RegisterAttached("ApplySortFixSorter", typeof(Comparison<object>), typeof(DataGridBehaviors), new PropertyMetadata(null, ApplySortFixSorterPropertyChanged));

        private static void ApplySortFixSorterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            DataGridColumn column = d as DataGridColumn;

            if (column == null)
                return;

            if (e.NewValue == null) {
                if ((column is DataGridTextColumn && column.SortMemberPath.IsMeaningful()) == false) {
                    column.CanUserSort = false;
                }
            }
            else {
                column.CanUserSort = true;
            }

        }

        public static void SetApplySortFix(DependencyObject obj, bool value) =>
            obj.SetValue(ApplySortFixProperty, value);

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

        private async static void DataGrid_Sorting(object sender, DataGridSortingEventArgs e) {
            var dataGrid = (DataGrid)sender;

            IList itemsSource = dataGrid.ItemsSource as IList;
            if (itemsSource == null)
                return;

            DataGridEnvironment environment = GetDataGridEnvironment(dataGrid);
            if (environment == null && itemsSource is INotifyCollectionChanged) {
                environment = new DataGridEnvironment(dataGrid);
                var notifyCollectionChanged = (INotifyCollectionChanged)itemsSource;
                notifyCollectionChanged.CollectionChanged += environment.CollectionChanged_CollectionChanged;
                SetDataGridEnvironment(dataGrid, environment);
            }

            DataGridColumn sortColumn = e.Column;
            // Check if column has command override for sort.
            ICommand overrideCommand = GetSortCommandOverride(sortColumn);
            if (overrideCommand != null) {
                e.Handled = true;
                environment?.DisableReset(true);

                var newSortDirection =
                    sortColumn.SortDirection == System.ComponentModel.ListSortDirection.Ascending ?
                        System.ComponentModel.ListSortDirection.Descending :
                        System.ComponentModel.ListSortDirection.Ascending;

                var parameter = new SortCommandOverrideParameter(GetSortCommandOverrideParameter(sortColumn), newSortDirection);

                if (overrideCommand is IAsyncCommand<SortCommandOverrideParameter>) {
                    await ((IAsyncCommand<SortCommandOverrideParameter>)overrideCommand).ExecuteAsync(parameter);
                }
                else if (overrideCommand is IAsyncCommand<object>) {
                    await ((IAsyncCommand<object>)overrideCommand).ExecuteAsync(parameter);
                }
                else {
                    overrideCommand.Execute(parameter);
                }

                sortColumn.SortDirection = newSortDirection;

                environment?.DisableReset(false);
                return;
            }

            // Normal operation resumed.
            OperationResult<ComparerFunctions> comparer = GetComparer(dataGrid, sortColumn);
            if (comparer.WasSuccessful) {
                e.Handled = true;
                environment?.DisableReset(true);
                NormalSort(dataGrid, sortColumn, comparer.Result, itemsSource);
                environment?.DisableReset(false);
                return;
            }
        }

        static void NormalSort(DataGrid dataGrid, DataGridColumn sortColumn, ComparerFunctions comparer, IList itemsSource) {
            dataGrid.IsEnabled = false;

            IEnumerable<object> sortedSelectedItems;
            IEnumerable<object> sortedAllItems;

            if (sortColumn.SortDirection == System.ComponentModel.ListSortDirection.Ascending) {
                sortedSelectedItems = dataGrid.SelectedItems.Cast<object>().ToArray().OrderByDescending(comparer.Selector, comparer.Comparer);
                sortedAllItems = dataGrid.Items.Cast<object>().ToArray().OrderByDescending(comparer.Selector, comparer.Comparer);

                sortColumn.SortDirection = System.ComponentModel.ListSortDirection.Descending;
            }
            else {
                sortedSelectedItems = dataGrid.SelectedItems.Cast<object>().ToArray().OrderBy(comparer.Selector, comparer.Comparer);
                sortedAllItems = dataGrid.Items.Cast<object>().ToArray().OrderBy(comparer.Selector, comparer.Comparer);

                sortColumn.SortDirection = System.ComponentModel.ListSortDirection.Ascending;
            }

            object selectedItem = dataGrid.SelectedItem;

            itemsSource.Clear();
            if (dataGrid.SelectionMode == DataGridSelectionMode.Extended)
                dataGrid.SelectedItems.Clear();
            dataGrid.SelectedItem = null;

            foreach (var item in sortedAllItems)
                itemsSource.Add(item);

            if (dataGrid.SelectionMode == DataGridSelectionMode.Extended) {
                foreach (var item in sortedSelectedItems)
                    dataGrid.SelectedItems.Add(item);
            }
            dataGrid.SelectedItem = selectedItem;

            dataGrid.IsEnabled = true;
        }

        static OperationResult<ComparerFunctions> GetComparer(DataGrid dataGrid, DataGridColumn sortColumn) {

            var comparer = GetApplySortFixSorter(sortColumn);
            if (comparer != null)
                return new OperationResult<ComparerFunctions>(new ComparerFunctions { Comparer = new ComparerCallback<object>(comparer), Selector = obj => obj });

            string sortParameter = sortColumn.SortMemberPath;

            if (sortParameter.IsMeaningful() == false && sortColumn is DataGridTextColumn) {
                Type columnType = sortColumn.GetType();
                PropertyInfo bindingProp = columnType.GetProperty("Binding");
                if (bindingProp == null)
                    return new OperationResult<ComparerFunctions>(false, null);

                var binding = bindingProp.GetValue(sortColumn) as Binding;
                if (binding == null)
                    return new OperationResult<ComparerFunctions>(false, null);

                sortParameter = binding.Path.Path;
            }

            if (sortParameter.IsMeaningful() == false)
                return new OperationResult<ComparerFunctions>(false, null);

            Type objectType = dataGrid.ItemsSource.GetType().GetGenericArguments().Single();
            PropertyInfo sortProperty = objectType.GetProperty(sortParameter);
            if (sortProperty == null)
                return new OperationResult<ComparerFunctions>(false, null);

            return new OperationResult<ComparerFunctions>(new ComparerFunctions { Comparer = Comparer<object>.Default, Selector = obj => sortProperty.GetValue(obj) });
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

            public void DisableReset(bool disable) =>
                _disabled = disable;

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

        private class ComparerFunctions {
            public IComparer<object> Comparer { get; set; }
            public Func<object, object> Selector { get; set; }
        }

        public static object GetRowDoubleClickCommandParameter(DependencyObject obj) =>
            obj.GetValue(RowDoubleClickCommandParameterProperty);

        public static void SetRowDoubleClickCommandParameter(DependencyObject obj, object value) =>
            obj.SetValue(RowDoubleClickCommandParameterProperty, value);

        // Using a DependencyProperty as the backing store for RowDoubleClickCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowDoubleClickCommandParameterProperty =
            DependencyProperty.RegisterAttached("RowDoubleClickCommandParameter", typeof(object), typeof(DataGridBehaviors), new PropertyMetadata(null));

        public static ICommand GetRowDoubleClickCommand(DependencyObject obj) =>
            (ICommand)obj.GetValue(RowDoubleClickCommandProperty);

        public static void SetRowDoubleClickCommand(DependencyObject obj, ICommand value) =>
            obj.SetValue(RowDoubleClickCommandProperty, value);

        // Using a DependencyProperty as the backing store for RowDoubleClickCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowDoubleClickCommandProperty =
            DependencyProperty.RegisterAttached("RowDoubleClickCommand", typeof(ICommand), typeof(DataGridBehaviors), new PropertyMetadata(null, RowDoubleClickCommand_PropertyChanged));

        private static void RowDoubleClickCommand_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is Control == false)
                return;

            var control = (Control)d;

            if (e.OldValue == null && e.NewValue != null) {
                control.MouseLeftButtonDown += DataGrid_MouseRowDoubleClick;
            }
            else if (e.OldValue != null && e.NewValue == null) {
                control.MouseLeftButtonDown -= DataGrid_MouseRowDoubleClick;
            }
        }

        private static void DataGrid_MouseRowDoubleClick(object sender, MouseButtonEventArgs e) {
            if (sender is DataGrid == false || e.ClickCount != 2)
                return;

            var dataGrid = (DataGrid)sender;
            var row = ItemsControl.ContainerFromElement(dataGrid, e.OriginalSource as DependencyObject) as DataGridRow;

            if (row == null)
                return;

            ICommand command = GetRowDoubleClickCommand(dataGrid);
            if (command == null)
                return;

            command.Execute(GetRowDoubleClickCommandParameter(row));
        }

        public static ICommand GetSortCommandOverride(DependencyObject obj) =>
            (ICommand)obj.GetValue(SortCommandOverrideProperty);

        public static void SetSortCommandOverride(DependencyObject obj, ICommand value) =>
            obj.SetValue(SortCommandOverrideProperty, value);

        // Using a DependencyProperty as the backing store for SortCommandOverride.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SortCommandOverrideProperty =
            DependencyProperty.RegisterAttached("SortCommandOverride", typeof(ICommand), typeof(DataGridBehaviors), new PropertyMetadata(null, SortCommandOverride_PropertyChanged));

        private static void SortCommandOverride_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            DataGridColumn column = d as DataGridColumn;

            if (column == null)
                return;

            if (e.NewValue == null) {
                if ((column is DataGridTextColumn && column.SortMemberPath.IsMeaningful()) == false) {
                    column.CanUserSort = false;
                }
            }
            else {
                column.CanUserSort = true;
                if (column is DataGridTextColumn == false)
                    column.SortMemberPath = "X";
            }

        }

        public static object GetSortCommandOverrideParameter(DependencyObject obj) =>
            obj.GetValue(SortCommandOverrideParameterProperty);

        public static void SetSortCommandOverrideParameter(DependencyObject obj, object value) =>
            obj.SetValue(SortCommandOverrideParameterProperty, value);

        // Using a DependencyProperty as the backing store for SortCommandOverrideParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SortCommandOverrideParameterProperty =
            DependencyProperty.RegisterAttached("SortCommandOverrideParameter", typeof(object), typeof(DataGridBehaviors), new PropertyMetadata(null));



    }
}