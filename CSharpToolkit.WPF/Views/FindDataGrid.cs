namespace CSharpToolkit.Views {
    using System;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Media;
    using CSharpToolkit.XAML.Behaviors;
    public class FindDataGrid : DataGrid {
        static FindDataGrid() { DefaultStyleKeyProperty.OverrideMetadata(typeof(FindDataGrid), new FrameworkPropertyMetadata(typeof(FindDataGrid))); }

        public FindDataGrid() {
            var resourceDictionary = new ResourceDictionary { Source = new Uri("CSharpToolkit;component/Resources/Primitives/Brushes.xaml", UriKind.Relative) };
            AlternatingRowBackground = (Brush)resourceDictionary["AlternatingRowBackgroundBrush"];
        }
    }
}
