namespace CSharpToolkit.Views {
    using System;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    public class FindView : FindViewBase {
        static FindView() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FindView), new FrameworkPropertyMetadata(typeof(FindView)));
        }

        public ObservableCollection<DataGridColumn> Columns => DataGrid.Columns;
        public Style RowStyle { get { return DataGrid.RowStyle; } set { DataGrid.RowStyle = value; } }

        public FindDataGrid DataGrid { get; set; } = new FindDataGrid();

        public DataGridSelectionMode SelectionMode {
            get { return (DataGridSelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectionMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register("SelectionMode", typeof(DataGridSelectionMode), typeof(FindView), new PropertyMetadata(DataGridSelectionMode.Extended, SelectionMode_PropertyChanged));

        private static void SelectionMode_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var dataGrid = ((FindView)d).DataGrid;
            if (dataGrid != null)
                dataGrid.SelectionMode = ((DataGridSelectionMode)e.NewValue);
        }
    }
}
