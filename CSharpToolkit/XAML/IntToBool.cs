namespace CSharpToolkit.XAML {
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows;
    public class IntToBool : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is int) {
                int intValue = ((int)value);
                switch(ComparisonFunction)  {
                    case ComparisonFunction.EqualTo:
                        if (intValue == ComparisonValue)
                            return IfTrueReturns;
                        return !IfTrueReturns;
                    case ComparisonFunction.NotEqualTo:
                        if (intValue != ComparisonValue)
                            return IfTrueReturns;
                        return !IfTrueReturns;
                    case ComparisonFunction.GreaterThan:
                        if (intValue > ComparisonValue)
                            return IfTrueReturns;
                        return !IfTrueReturns;
                    case ComparisonFunction.LessThan:
                        if (intValue < ComparisonValue)
                            return IfTrueReturns;
                        return !IfTrueReturns;
                    case ComparisonFunction.GreaterThanOrEqualTo:
                        if (intValue >= ComparisonValue)
                            return IfTrueReturns;
                        return !IfTrueReturns;
                    case ComparisonFunction.LessThanOrEqualTo:
                        if (intValue <= ComparisonValue)
                            return IfTrueReturns;
                        return !IfTrueReturns;
                }
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }

        public int ComparisonValue { get; set; } = 0;
        public ComparisonFunction ComparisonFunction { get; set; } = ComparisonFunction.EqualTo;
        public bool IfTrueReturns { get; set; } = true;
    }
}