namespace CSharpToolkit.DataAccess.Abstractions {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CSharpToolkit.DataAccess;
    using CSharpToolkit.DataAccess.Abstractions;
    using CSharpToolkit.Utilities;
    using System.Reflection;
    using DataAccess.MetaData;
    using CSharpToolkit.Extensions;

    public abstract class AdminQueryProviderBase<TModel> : ISimpleAdminQueryProvider<TModel> {

        static AdminQueryProviderBase() {
            Tuple<TableNameAttribute, PropertyInfo> info = MetaDataOperations.GetTableNameAndIdTuple<TModel>();
            TableName = info.Item1.TableName;
            IdProperty = info.Item2;
        }

        public static string TableName { get; }
        public static PropertyInfo IdProperty { get; }

        public virtual IAliasedCommandTypeDataOrder Create(TModel model) {
            var dataOrder = Get.InsertQuery(TableName, typeof(TModel).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(prop => prop.Name != IdProperty.Name && prop.CanRead).Select(prop => new KeyValuePair<string, object>(prop.Name, prop.GetValue(model))), new[] { IdProperty.Name });
            return new AliasedCommandTypeDataOrder(dataOrder, System.Data.CommandType.Text);
        }

        public virtual IAliasedCommandTypeDataOrder Create() {
            var dataOrder = Get.InsertQuery(TableName, new KeyValuePair<string, object>[0], new[] { IdProperty.Name });
            return new AliasedCommandTypeDataOrder(dataOrder, System.Data.CommandType.Text);
        }

        public virtual IAliasedCommandTypeDataOrder Delete(TModel model) {
            Clause whereCondition =
                Clause.New()
                    .AddClause(IdProperty.GetValue(model), Clause.EqualStatementCallback(IdProperty.Name));

            var dataOrder = Get.DeleteQuery(TableName, whereCondition);
            return new AliasedCommandTypeDataOrder(dataOrder, System.Data.CommandType.Text);
        }

        public virtual IAliasedCommandTypeDataOrder Update(TModel model) {
            if (model == null)
                return new AliasedCommandTypeDataOrder();

            IEnumerable<KeyValuePair<string, object>> propertiesLinqStatement;

            if (model is IChangeDescriptor) {
                propertiesLinqStatement =
                    ((IChangeDescriptor)model)
                        .GetChangedProperties()
                        .Where(p => p.PropertyName != IdProperty.Name)
                        .Select(p => new KeyValuePair<string, object>(p.PropertyName, p.NewValue));
            }
            else {
                propertiesLinqStatement =
                    typeof(TModel)
                    .GetProperties()
                    .Where(p => p.Name != IdProperty.Name)
                    .Select(p => new KeyValuePair<string, object>(p.Name, p.GetValue(model)));
            }

            KeyValuePair<string, object>[] properties = propertiesLinqStatement.ToArray();

            Clause whereCondition =
                Clause.New()
                    .AddClause(IdProperty.GetValue(model), Clause.EqualStatementCallback(IdProperty.Name));

            var dataOrder = Get.UpdateQuery(TableName, properties, whereCondition);
            var updatePrependageOrder = GetUpdatePrependage(properties);

            string query = string.Join(";\r\n\r\n", new[] { updatePrependageOrder?.Query ?? "", dataOrder.Query }.Where(q => q.IsMeaningful()));
            var parameters = new List<KeyValuePair<string, object>>(dataOrder.Parameters);
            parameters.AddRange(updatePrependageOrder?.Parameters ?? new KeyValuePair<string, object>[0]);

            return new AliasedCommandTypeDataOrder(query, System.Data.CommandType.Text, parameters);
        }

        public abstract IAliasedCommandTypeDataOrder Lookup(TModel model);

        public virtual ISimpleDataOrder GetUpdatePrependage(IEnumerable<KeyValuePair<string, object>> properties) =>
            new SimpleDataOrder();
    }
}
