namespace CSharpToolkit.ViewModels.Abstractions {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CSharpToolkit.Utilities;
    using CSharpToolkit.XAML;
    using Validation;
    using Validation.Abstractions;
    using System.Threading.Tasks;

    public abstract class ValidationBase : EntityBase {

        static Task DefaultValidationCallback(string columnName, object value, List<Tuple<string, OperationResult>> originalValidation) =>
            Task.CompletedTask;

        bool _modifying;
        bool _disposed;
        Func<string, object, List<Tuple<string, OperationResult>>, Task> _runValidationCallback;

        public bool Modifying {
            get { return _modifying; }
            set {
                if (Perform.ReplaceIfDifferent(ref _modifying, value).WasSuccessful) {
                    // If modifying has ended, clear all errors. 
                    if (_modifying == false) {
                        foreach (var error in Errors.Select(e => e.Item1).ToArray())
                            ClearErrors(error);
                    }
                    OnPropertyChanged();
                }
            }
        }

        public Func<string, object, List<Tuple<string, OperationResult>>, Task> RunValidationCallback {
            get { return _runValidationCallback ?? DefaultValidationCallback; }
            set { FirePropertyChangedIfDifferent(ref _runValidationCallback, value); }
        }

        public override string this[string columnName] {
            get {
                RunValidation(columnName);
                return base[columnName];
            }
        }

        protected virtual Task<List<Tuple<string, OperationResult>>> RunValidation(string columnName, object value) =>
            Task.FromResult(new List<Tuple<string, OperationResult>>());

        public async Task RunValidation(string columnName) {
            ClearErrors(columnName);
            if (Modifying == false)
                return;

            var prop = GetType().GetProperty(columnName);
            if (prop == null)
                return;

            object value = prop.GetValue(this);
            var validationResults = new List<Tuple<string, OperationResult>>(await RunValidation(columnName, value));
            await RunValidationCallback(columnName, value, validationResults);

            foreach (var group in validationResults.GroupBy(results => results.Item1)) {
                if (group.Any(g => g.Item2.HadErrors))
                    AddErrors(group.Key, group.SelectMany(result => result.Item2.Exceptions.Select(e => e.Message)));
                else
                    ClearErrors(group.Key);
            }
        }

        protected override void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    RunValidationCallback = null;
                }
                _disposed = true;
            }

            base.Dispose(disposing);
        }

    }
}