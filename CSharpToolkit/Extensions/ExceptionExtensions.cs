using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpToolkit.Extensions {
    public static class ExceptionExtensions {

        public static void AttachInfo(this IEnumerable<Exception> exception, string name, object data) =>
            AttachInfo(exception, new[] { Tuple.Create(name, data) });

        public static void AttachInfo(this Exception exception, string name, object data) =>
            AttachInfo(new[] { exception }, new[] { Tuple.Create(name, data) });

        public static void AttachInfo(this Exception exception, Tuple<string, object> info) =>
            AttachInfo(new[] { exception }, new[] { info });

        public static void AttachInfo(this Exception exception, IEnumerable<Tuple<string, object>> info) =>
            AttachInfo(new[] { exception }, info);

        public static void AttachInfo(this IEnumerable<Exception> exception, Tuple<string, object> info) =>
            AttachInfo(exception, new[] { info });

        public static void AttachInfo(this IEnumerable<Exception> exceptions, IEnumerable<Tuple<string, object>> info) {
            exceptions = exceptions ?? Enumerable.Empty<Exception>();
            info = info ?? Enumerable.Empty<Tuple<string, object>>();
            DateTime now = DateTime.Now;
            exceptions.ForEach(ex => info.ForEach(i => {
                try {
                    ex.Data.Add(i.Item1, i.Item2);
                }
                catch {
                    ex.Data.Add(i.Item1 + now.ToString("yyyyMMddHHmmssfff"), i.Item2);
                }
            }));
        }

    }
}
