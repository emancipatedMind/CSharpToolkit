namespace CSharpToolkit.XAML {
    using Abstractions;
    using EventArgs;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.ComponentModel.DataAnnotations;

    public abstract class EntityBase : IEntityBase {
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        public IEnumerable GetErrors(string propertyName) {
            if (string.IsNullOrEmpty(propertyName))
                return _errors.Values;
            return _errors.ContainsKey(propertyName) ? _errors[propertyName] : null;
        }

        public bool HasErrors => _errors.Count != 0;


        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        protected void OnErrorsChanged(string propertyName) {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        protected void ClearErrors(string propertyName = "") {
            bool fireEvent = HasErrors;
            _errors.Remove(propertyName);
            OnErrorsChanged(propertyName);
            if (fireEvent) OnPropertyChanged(nameof(HasErrors));
        }

        protected void AddError(string propertyName, string error) {
            AddErrors(propertyName, new List<string> { error });
        }

        protected void AddErrors(string propertyName, IList<string> errors) {
            bool fireEvent = HasErrors == false;
            bool changed = false;
            if (!_errors.ContainsKey(propertyName)) {
                _errors.Add(propertyName, new List<string>());
                changed = true;
            }
            errors.ToList().ForEach(x => {
                if (_errors[propertyName].Contains(x)) return;
                _errors[propertyName].Add(x);
                changed = true;
            });
            if (changed) {
                OnErrorsChanged(propertyName);
                if (fireEvent) OnPropertyChanged(nameof(HasErrors));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "" ) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event EventHandler<GenericEventArgs<string>> Notify;
        protected void FireNotifyEvent(GenericEventArgs<string> e) =>
            Notify?.Invoke(this, e);
        protected void FireNotifyEvent(string notification) =>
            Notify?.Invoke(this, new GenericEventArgs<string>(notification));

        public virtual string Error { get; }
        public virtual string this[string columnName] => "";

        protected string[] GetErrorsFromAnnotations<T>(string propertyName, T value) {
            var results = new List<ValidationResult>();
            var vc = new ValidationContext(this, null, null) { MemberName = propertyName };
            var isValid = Validator.TryValidateProperty(value, vc, results);
            return isValid ? null : Array.ConvertAll(results.ToArray(), o => o.ErrorMessage);
        }
    }
}
