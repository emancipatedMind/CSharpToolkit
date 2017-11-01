namespace CSharpToolkit.Extensions {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    public static class EnumerableExtensions {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
            foreach (var s in source)
                action(s);
        }
        //public static IEnumerable<(T item, int index)> WithIndices<T>(this IEnumerable<T> source) =>
        //    source.Select((s, i) => (s, i));
    }
}
