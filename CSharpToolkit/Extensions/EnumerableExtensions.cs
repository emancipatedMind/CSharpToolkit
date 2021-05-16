namespace CSharpToolkit.Extensions {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    /// <summary>
    /// Extensions for IEnumerable.
    /// </summary>
    public static class EnumerableExtensions {
        /// <summary>
        /// Provides ForEach method for IEnumerableTypes.
        /// </summary>
        /// <typeparam name="T">Source type.</typeparam>
        /// <param name="source">Source object for operation.</param>
        /// <param name="action">Method to be performed using each item.</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
            foreach (var s in source)
                action(s);
        }
    }
}
