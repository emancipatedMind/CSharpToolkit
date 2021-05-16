namespace CSharpToolkit.ViewModels {
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Abstractions;
    using CSharpToolkit.Utilities;
    using CSharpToolkit.Utilities.EventArgs;
    using Utilities;

    public class SingleFieldViewModel<TField> : DialogViewModel<GenericEventArgs<TField>> {

        private TField _field;

        public CSharpToolkit.Validation.Abstractions.IValidate<TField> Validator { get; set; }
        public virtual TField Field {
            get { return _field; }
            set { FirePropertyChangedIfDifferent(ref _field, value); }
        }

        public override string this[string columnName] {
            get {
                ClearErrors(columnName);

                if (Validator != null && columnName == nameof(Field)) {
                    OperationResult operation = Validator.Validate(Field);

                    if (operation.HadErrors)
                        AddErrors(columnName, operation.Exceptions.Select(ex => ex.Message));
                }

                return "";
            }
        }

        protected override GenericEventArgs<TField> GetSuccessObject() =>
            new GenericEventArgs<TField>(Field);

    }
}
