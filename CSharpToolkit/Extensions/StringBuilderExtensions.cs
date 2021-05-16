namespace CSharpToolkit.Extensions {
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    public static class StringBuilderExtensions {

        public static StringBuilder AppendLineStringJoin(this StringBuilder builder, string separator, IEnumerable<string> values) {
            separator = separator == null ? "" : separator;

            builder.Append(string.Join(separator, values?.ToArray() ?? new string[0]));
            return builder;
        }

    }
}
