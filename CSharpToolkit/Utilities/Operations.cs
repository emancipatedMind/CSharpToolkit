namespace CSharpToolkit.Utilities {
    using System;
    using System.Collections.Generic;
    public static class Operations {

        [Obsolete("Use Perform static class.")]
        public static OperationResult PerformReplaceIfDifferent<T>(ref T oldValue, T newValue, T defaultValue = default(T)) {
            if (newValue == null) {
                if (defaultValue == null)
                    return new OperationResult(false);
                else
                    newValue = defaultValue;
            }

            if (oldValue?.Equals(newValue) == true)
                return new OperationResult(false);

            oldValue = newValue;
            return new OperationResult(true);
        }

    }
}