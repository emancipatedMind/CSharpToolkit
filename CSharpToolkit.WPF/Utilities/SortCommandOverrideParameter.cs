namespace CSharpToolkit.Utilities {
    using System.ComponentModel;
    public class SortCommandOverrideParameter {

        public SortCommandOverrideParameter(object parameter, ListSortDirection sortDirection) {
            Parameter = parameter;
            SortDirection = sortDirection;
        }

        public object Parameter { get; }
        public ListSortDirection SortDirection { get; }

    }
}
