namespace CSharpToolkit.XAML {
    using System.Linq;
    using Validation.Abstractions;
    public class SingleValueViewModel<T> : EntityBase {

        T _value;

        public SingleValueViewModel(T value) : this(value, null) { }
        public SingleValueViewModel(T value, IValidate<T> validator) {
            _value = value;
            Validator = validator;
        }

        public IValidate<T> Validator { get; set; }

        public T Value {
            get { return _value; }
            set { FirePropertyChangedIfDifferent(ref _value, value); }
        }

        public override string this[string columnName] {
            get {
                ClearErrors(columnName);
                if (Validator != null) {
                    var validation = Validator.Validate(Value);
                    if (validation.WasSuccessful == false) {
                        AddErrors(columnName, validation.Exceptions.Select(ex => ex.Message).ToArray()); 
                    }
                } 

                return base[columnName];
            }
        }

    }
}
