namespace CSharpToolkit.Utilities {
    using System;
    using System.Collections.Generic;
    /// <summary>
    /// Operations for use.
    /// </summary>
    public static class Operations {

        /// <summary>
        /// Performs a replace if the new, and old value are different. If new value is null, default value is consulted. If it is also null, no swap is performed.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value for variable.</param>
        /// <param name="defaultValue">Default value for use.</param>
        /// <returns>Operation result denoting whether value was updated.</returns>
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