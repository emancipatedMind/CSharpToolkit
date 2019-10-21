namespace CSharpToolkit.XAML {
    using Abstractions;
    using Utilities;
    using Utilities.Abstractions;
    using Utilities.EventArgs;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.ComponentModel.DataAnnotations;
    using System.Windows.Input;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Class used as base for ViewModel providing common ViewModel operations.
    /// </summary>
    public abstract class EntityBase : IUserNotifier, INotifyDataErrorInfo, INotifyPropertyChanged, IDataErrorInfo, IExplicitErrorAdder, INotifyDisposable, IFocusChanger {
        private bool disposedValue = false;

        readonly Dictionary<string, ObservableCollection<string>> _errors = new Dictionary<string, ObservableCollection<string>>();
        readonly ObservableCollection<Tuple<string, ReadOnlyObservableCollection<string>>> _publicErrors = new ObservableCollection<Tuple<string, ReadOnlyObservableCollection<string>>>();

        public EntityBase() {
            Errors = new ReadOnlyObservableCollection<Tuple<string, ReadOnlyObservableCollection<string>>>(_publicErrors);
        }

        /// <summary>
        /// Public list of errors for binding. Cannot be modified directly.
        /// </summary>
        public ReadOnlyObservableCollection<Tuple<string, ReadOnlyObservableCollection<string>>> Errors { get; }

        /// <summary>
        /// Gets errors recorded by Property.
        /// </summary>
        /// <param name="propertyName">Property name for which errors are requested. Empty string returns all errors.</param>
        /// <returns>Errors found for propertyName.</returns>
        public IEnumerable GetErrors(string propertyName) {
            if (string.IsNullOrEmpty(propertyName))
                return _publicErrors.Select(e => e.Item2);
            return _publicErrors.FirstOrDefault(e => e.Item1 == propertyName)?.Item2;
        }

        /// <summary>
        /// Whether any errors have been recorded.
        /// </summary>
        public bool HasErrors => _errors.Any();

        /// <summary>
        /// Raised whenever errors are added or removed.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Raise <see cref="ErrorsChanged"/> event.
        /// </summary>
        protected void OnErrorsChanged(string propertyName) =>
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

        /// <summary>
        /// Clears errors for Property;
        /// </summary>
        /// <param name="propertyName">Property name for which errors should be cleared.</param>
        protected void ClearErrors(string propertyName = "") {
            bool fireEvent = HasErrors;
            if (_errors.ContainsKey(propertyName)) {
                _errors.Remove(propertyName);
                var item = _publicErrors.FirstOrDefault(er => er.Item1 == propertyName);
                if (item != null)
                    _publicErrors.Remove(item);
            }
            OnErrorsChanged(propertyName);
            if (fireEvent) OnPropertyChanged(nameof(HasErrors));
        }

        /// <summary>
        /// Add error.
        /// </summary>
        /// <param name="propertyName">Property name for which errors should be added.</param>
        /// <param name="error">Error text.</param>
        protected void AddError(string propertyName, string error) =>
            AddErrors(propertyName, new[] { error });

        /// <summary>
        /// Add errors.
        /// </summary>
        /// <param name="propertyName">Property name for which errors should be added.</param>
        /// <param name="errors">Error text to add.</param>
        protected void AddErrors(string propertyName, IEnumerable<string> errors) {
            if ((errors?.Any() ?? false) == false)
                return;

            bool hasErrorsHasChanged = HasErrors == false;
            bool incomingPropertyErrorsHasChanged = false;

            bool listContainsKey = _errors.ContainsKey(propertyName);
            ObservableCollection<string> list;

            if (listContainsKey) {
                list = _errors[propertyName];
            }
            else {
                list = new ObservableCollection<string>();
                _errors.Add(propertyName, list);
                _publicErrors.Add(Tuple.Create(propertyName, new ReadOnlyObservableCollection<string>(list)));
                incomingPropertyErrorsHasChanged = true;
            }

            foreach (var error in errors) {
                if (list.Contains(error)) 
                    continue;

                list.Add(error);
                incomingPropertyErrorsHasChanged = true;
            }

            if (incomingPropertyErrorsHasChanged) {
                OnErrorsChanged(propertyName);
                if (hasErrorsHasChanged) OnPropertyChanged(nameof(HasErrors));
            }
        }

        /// <summary>
        /// Fired whenever an important property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Signal that property has changed. If no argument is provided, and called within getter or setter for Property, that property name is passed automatically.
        /// </summary>
        /// <param name="propertyName">The name of the property to signal that has changed.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "" ) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// A helper method that will simply assign newValue to field if they are different, and fire PropertyChanged event with propertyName. If propertyName not supplied, Caller's name will be used. 
        /// </summary>
        /// <typeparam name="TProperty">Value type.</typeparam>
        /// <param name="field">The field for which swap will be performed.</param>
        /// <param name="newValue">The new value to check.</param>
        /// <param name="defaultValue">Default value for use if newValue is null.</param>
        /// <param name="callerMemberName">The name of the property to signal that has changed.</param>
        protected void FirePropertyChangedIfDifferent<TProperty>(ref TProperty field, TProperty newValue, TProperty defaultValue = default(TProperty),[CallerMemberName] string callerMemberName = "") {
            if (Perform.ReplaceIfDifferent(ref field, newValue, defaultValue).WasSuccessful) {
                OnPropertyChanged(callerMemberName);
            }
        }

        /// <summary>
        /// Performs a replace of a property using the propertyName on the obj provided. If new value is null, default value is consulted. If it is also null, this is what will be assigned.
        /// </summary>
        /// <typeparam name="TProperty">Value type.</typeparam>
        /// <param name="obj">The object to use during replace.</param>
        /// <param name="propertyName">The property name to use for replace.</param>
        /// <param name="newValue">New value for variable.</param>
        /// <param name="defaultValue">Default value for use if newValue is null.</param>
        /// <param name="callerMemberName">The name of the property to signal that has changed.</param>
        /// <returns>Operation result denoting whether value was updated.</returns>
        protected void FirePropertyChangedIfDifferent<TProperty>(object obj, string propertyName, TProperty newValue, TProperty defaultValue = default(TProperty),[CallerMemberName] string callerMemberName = "") {
            if (Perform.ReplaceIfDifferent(obj, propertyName, newValue, defaultValue).WasSuccessful) {
                OnPropertyChanged(callerMemberName);
            }
        }

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
        /// Simple error text. Binding for property must have <see cref="System.Windows.Data.Binding.ValidatesOnDataErrors"/> set to true.
        /// </summary>
        public virtual string Error { get; } = "";

        /// <summary>
        /// Indexer for use during validation. Binding for property must have <see cref="System.Windows.Data.Binding.ValidatesOnDataErrors"/> set to true.
        /// </summary>
        /// <param name="columnName">Column for validation.</param>
        /// <returns>Error found from validation.</returns>
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

        void IExplicitErrorAdder.AddError(string propertyName, Exception ex) {
            if (GetType().GetProperties().Select(prop => prop.Name).Contains(propertyName) == false)
                return;

            AddError(propertyName, ex.Message);
        }

        public event EventHandler<GenericEventArgs<FocusNavigationDirection>> FocusChangeRequested;
        public void RequestFocusChange(FocusNavigationDirection request) {
            FocusChangeRequested?.Invoke(this, new GenericEventArgs<FocusNavigationDirection>(request));
        }
        public void RequestNextFocus() =>
            RequestFocusChange(FocusNavigationDirection.Next);

        public void RequestPreviousFocus() =>
            RequestFocusChange(FocusNavigationDirection.Previous);

        #region IDisposable Support
        /// <summary>
        /// Notification that disposing has finished.
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        /// Called to complete disposing.
        /// </summary>
        /// <param name="disposing">Whether or not managed resources are released.</param>
        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    foreach(var item in _errors) {
                        item.Value.Clear();
                    }
                    _errors.Clear();
                    _publicErrors.Clear();

                    ErrorsChanged = null;
                    Notify = null;
                    PropertyChanged = null;
                    FocusChangeRequested = null;
                }
                disposedValue = true;
                Disposed?.Invoke(this, EventArgs.Empty);
            }
            Disposed = null;
        }

        // This code added to correctly implement the disposable pattern.
        /// <summary>
        /// Used to release resources in use by the object.
        /// </summary>
        public void Dispose() {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}