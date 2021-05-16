namespace CSharpToolkit.DataAccess {
    using Abstractions;
    using CSharpToolkit.DataAccess.Abstractions;
    using CSharpToolkit.DataAccess;
    using CSharpToolkit.Utilities;
    using System.Linq;
    public class SimpleAdminQueryProvider<TModel> : AdminQueryProviderBase<TModel>, ISimpleAdminQueryProvider<TModel> {
        public override IAliasedCommandTypeDataOrder Lookup(TModel model) {

            Clause whereClause =
                Clause
                    .New()
                    .AddClause(
                        IdProperty.GetValue(model),
                        Clause.EqualStatementCallback(IdProperty.Name)
                    );

            var fields = typeof(TModel).GetProperties().Select(p => p.Name);

            var selectQuery =
                SelectQuery
                    .New(TableName, fields, false)
                    .Build(whereClause);

            return new AliasedCommandTypeDataOrder(selectQuery, System.Data.CommandType.Text);
        }

    }
}
