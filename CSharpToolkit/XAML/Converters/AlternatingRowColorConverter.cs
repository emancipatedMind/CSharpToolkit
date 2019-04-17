namespace CSharpToolkit.XAML.Converters {
    using System;
    using System.Globalization;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    /// <summary>
    /// Provides a converter that will alternate row colors on a ListView.
    /// </summary>
    public class AlternatingRowColorConverter : IValueConverter {

        /// <summary>
        /// Used to alternate row colors on a ListView.
        /// </summary>
        /// <param name="value">Input value.</param>
        /// <param name="targetType">Output value type. (Unused.)</param>
        /// <param name="parameter">Converter parameter. (Unused.)</param>
        /// <param name="culture">Culture. (Unused.)</param>
        /// <returns>If index of listviewitem is divisible evenly by 2, then SecondRow is the background color. Otherwise, FirstRow is the color.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            ListViewItem item = (ListViewItem)value;
            ListView listView = ItemsControl.ItemsControlFromItemContainer(item) as ListView;
            int index = listView.ItemContainerGenerator.IndexFromContainer(item);
            return index % 2 == 0 ? FirstRow : SecondRow;
        }

        /// <summary>
        /// Method Not Implemented. Returns <paramref name="value"/> if invoked.
        /// </summary>
        /// <param name="value">Method Not Implemented. Returns <paramref name="value"/> if invoked.</param>
        /// <param name="targetType">Method Not Implemented. Returns <paramref name="value"/> if invoked.</param>
        /// <param name="parameter">Method Not Implemented. Throws exception if invoked.</param>
        /// <param name="culture">Method Not Implemented. Throws exception if invoked.</param>
        /// <returns>Not Implemented. Throws exception if invoked.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Background color for odd colored rows. White by default.
        /// </summary>
        public Brush FirstRow { get; set; } = Brushes.White;
        /// <summary>
        /// Background color for even colored rows. Orange by default.
        /// </summary>
        public Brush SecondRow { get; set; } = Brushes.Orange;
    }
}