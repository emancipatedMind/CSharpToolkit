namespace CSharpToolkit.Utilities {
    using Abstractions;
    using DataAccess;
    using System.Linq;
    public class SecondaryQuerySource : ISecondaryQuerySource {

        public SecondaryQuerySource(string tableName, string tableAlias, ) 

        public string[] Columns { get; set; }

        public Clause JoinClause { get; set; }

        public string TableAlias { get; set; }

        public string TableName { get; set; }

        public SecondaryQuerySource MakeDeepCopy() =>
            new SecondaryQuerySource {
                Columns = Columns.ToArray(),
                JoinClause = JoinClause,
                TableAlias = TableAlias,
                TableName = TableName,
            };

        internal static SecondaryQuerySource Convert(SelectQueryOrder order) {
            Clause clause =
                Clause.New()
                .AddClause($"{(string.IsNullOrWhiteSpace(order.TableAlias) ? order.TableName : order.TableAlias)}.{order.JoinColumn} = {order.ForeignKey.Key}.{order.ForeignKey.Value}");

            return new SecondaryQuerySource {
                Columns = order.Columns ?? new string[0],
                JoinClause = clause,
                TableAlias = order.TableAlias,
                TableName = order.TableName,
            };
        }

    }
}