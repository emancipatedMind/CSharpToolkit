namespace CSharpToolkit.XAML.Behaviors {
    using System;
    public class CallbackListItemConverter : Abstractions.IListItemConverter {

        Func<object, object> _convert;
        Func<object, object> _convertBack;

        public CallbackListItemConverter(Func<object, object> convert = null, Func<object, object> convertBack = null) {
            _convert = convert ?? new Func<object, object>(o => o);
            _convertBack = convertBack ?? new Func<object, object>(o => o);
        }

        public object Convert(object item) => _convert(item);
        public object ConvertBack(object item) => _convertBack(item);

    }
}
