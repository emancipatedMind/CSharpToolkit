namespace CSharpToolkit.Utilities.Formatters {
    using System;
    using System.Linq;
    using Abstractions;
    using DataAccess.Abstractions;

    /// <summary>
    /// Formats the <see cref="IAliasedCommandTypeDataOrder"/> into valid T-Sql.
    /// </summary>
    public class TSqlFormatter : IAliasedCommandTypeDataOrderFormatter {

        /// <summary>
        /// Whether log should treat DateTime as DateTime2.
        /// </summary>
        public bool UseDateTime2 { get; set; }

        /// <summary>
        /// Formats the incoming <see cref="IAliasedCommandTypeDataOrder"/> into valid T-Sql.
        /// </summary>
        /// <param name="order">The <see cref="IAliasedCommandTypeDataOrder"/> to format.</param>
        /// <returns>The data order formatted into valid T-Sql.</returns>
        public string Format(IAliasedCommandTypeDataOrder order) =>
            Use.StringBuilder(builder => {
                builder.AppendLine($"Query type : {order.CommandType}");
                if (order.Parameters.Any()) {
                    builder.AppendLine();
                    foreach (var parameter in order.Parameters) {
                        string sqlValueType = "<unknown>";
                        object value = parameter.Value;
                        if (value == null || value is DBNull) {
                            value = null;
                            sqlValueType = $"varchar(1)";
                        }

                        else if (value is string) {
                            string valueAsString = ((string)value).Replace("'", "''");
                            sqlValueType = $"varchar({(valueAsString.Length > 0 ? valueAsString.Length : 1)})";
                            value = $"'{valueAsString}'";
                        }

                        else if (value is DateTime || value is DateTime?) {
                            sqlValueType = "datetime2";
                            DateTime time = ((DateTime)value);

                            if (UseDateTime2 == false) {
                                sqlValueType = "datetime";
                                int additive = 1;
                                int leastSignificantDigit = time.Millisecond % 10;
                                if (leastSignificantDigit < 2)
                                    additive = 0 - leastSignificantDigit;
                                else if (leastSignificantDigit < 5)
                                    additive = 3 - leastSignificantDigit;
                                else if (leastSignificantDigit < 9)
                                    additive = 7 - leastSignificantDigit;

                                time = new DateTime((time.Ticks / 10000 + additive) * 10000);

                                value = $"'{time.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture)}'";
                            }
                        }

                        else if (value is int || value is int?)
                            sqlValueType = "int";

                        else if (value is decimal || value is decimal?)
                            sqlValueType = "decimal";

                        else if (value is System.Xml.Linq.XDocument) {
                            sqlValueType = "xml";
                            value = $"'{Environment.NewLine}{((System.Xml.Linq.XDocument)value).ToString(System.Xml.Linq.SaveOptions.None)}{Environment.NewLine}'";
                        }

                        builder.AppendLine($"DECLARE {parameter.Key} {sqlValueType} = {value ?? "NULL" };");
                    }
                }
                builder.AppendLine();
                if (order.CommandType == System.Data.CommandType.StoredProcedure) {
                    builder.Append("EXEC ");
                }
                builder.AppendLine(order.Query);
                // Puts Lines number in query.
                //builder.AppendLine(string.Join("\r\n", order.Query.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Select((line, index) => $"-- {index + 1}:\r\n{line}")));
            });
    }
}