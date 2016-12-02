namespace CSharpToolkit.XAML {
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class BooleanNotConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if ((value is bool) && ((bool)value) == false) return true;
            else return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}