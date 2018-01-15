namespace CSharpToolkit.XAML {
    using Abstractions;
    using Utilities.Abstractions;
    using EventArgs;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.ComponentModel.DataAnnotations;
    /// <summary>
    /// Class used as base for ViewModel providing common ViewModel operations.
    /// </summary>
    public abstract class EntityBase : IEntityBase {
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        /// <summary>
        /// Gets errors recorded by Property.
        /// </summary>
        /// <param name="propertyName">Property name for which errors are requested. Empty string returns all errors.</param>
        /// <returns>Errors found for propertyName.</returns>
        public IEnumerable GetErrors(string propertyName) {
            if (string.IsNullOrEmpty(propertyName))
                return _errors.Values;
            return _errors.ContainsKey(propertyName) ? _errors[propertyName] : null;
        }

        /// <summary>
        /// Whether any errors have been recorded.
        /// </summary>
        public bool HasErrors => _errors.Count != 0;

        /// <summary>
        /// Raised whenever errors are added or removed.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        /// <summary>
        /// Raise ErrorsChanged event.
        /// </summary>
        protected void OnErrorsChanged(string propertyName) =>
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

        /// <summary>
        /// Clears errors for Property;
        /// </summary>
        /// <param name="propertyName">Property name for which errors should be cleared.</param>
        protected void ClearErrors(string propertyName = "") {
            bool fireEvent = HasErrors;
            _errors.Remove(propertyName);
            OnErrorsChanged(propertyName);
            if (fireEvent) OnPropertyChanged(nameof(HasErrors));
        }

        /// <summary>
        /// Add error.
        /// </summary>
        /// <param name="propertyName">Property name for which errors should be added.</param>
        /// <param name="error">Error text.</param>
        protected void AddError(string propertyName, string error) =>
            AddErrors(propertyName, new List<string> { error });

        /// <summary>
        /// Add errors.
        /// </summary>
        /// <param name="propertyName">Property name for which errors should be added.</param>
        /// <param name="errors">Error text to add.</param>
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

        /// <summary>
        /// Fired whenever an important property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Signal that property has changed. If no argument is provided, and called within getter or setter for Property, that property name is passed automatically.
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "" ) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        /// <summary>
        /// Signal that all properties have changed. Analogous to calling OnPropertyChanged with an empty string.
        /// </summary>
        public void SignalAllPropertiesChanged() =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));

        /// <summary>
        /// Fired whenever a notification needs to be given.
        /// </summary>
        public event EventHandler<GenericEventArgs<string, Urgency>> Notify;
        /// <summary>
        /// Fire notify event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected void FireNotifyEvent(GenericEventArgs<string, Urgency> e) =>
            Notify?.Invoke(this, e);
        /// <summary>
        /// Fire notify event.
        /// </summary>
        /// <param name="notification">Notification text.</param>
        /// <param name="urgency">Denotes urgency of message.</param>
        protected void FireNotifyEvent(string notification, Urgency urgency) =>
            Notify?.Invoke(this, new GenericEventArgs<string, Urgency>(notification, urgency));

        /// <summary>
        /// Simple error text. Binding for property must have ValidatesOnDataErrors set to true.
        /// </summary>
        public virtual string Error { get; }
        /// <summary>
        /// Indexer for use during validation. Binding for property must have ValidatesOnDataErrors set to true.
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public virtual string this[string columnName] => "";

        /// <summary>
        /// Validate field using attributes attached to property.
        /// </summary>
        /// <typeparam name="T">Property type.</typeparam>
        /// <param name="propertyName">Property Name.</param>
        /// <param name="value">Property Value.</param>
        /// <returns>Errors found.</returns>
        protected string[] GetErrorsFromAnnotations<T>(string propertyName, T value) {
            var results = new List<ValidationResult>();
            var vc = new ValidationContext(this, null, null) { MemberName = propertyName };
            var isValid = Validator.TryValidateProperty(value, vc, results);
            return isValid ? new string[0] : Array.ConvertAll(results.ToArray(), o => o.ErrorMessage);
        }
    }
}