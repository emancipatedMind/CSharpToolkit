namespace CSharpToolkit.Diagnostics {
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    public class ReflectionServices {
        public Dictionary<string, string> QuickViewProperties(object obj) =>
            obj
                .GetType()
                .GetProperties()
                .Select(p => p.GetGetMethod())
                .Where(m => m.IsPublic)
                .Aggregate(new Dictionary<string, string>(), (d, m) => {
                    d.Add(m.Name, m.Invoke(obj, null).ToString());
                    return d;
                });
    }
}
