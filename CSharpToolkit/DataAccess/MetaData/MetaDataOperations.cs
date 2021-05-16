namespace CSharpToolkit.DataAccess.MetaData {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    public static class MetaDataOperations {

        public static Tuple<TableNameAttribute, PropertyInfo> GetTableNameAndIdTuple<TModel>() =>
            GetTableNameAndIdTuple(typeof(TModel));

        public static Tuple<TableNameAttribute, PropertyInfo> GetTableNameAndIdTuple(Type type) {
            var tableNameAttribute = type.GetCustomAttributes(typeof(TableNameAttribute), true).FirstOrDefault() as TableNameAttribute;
            if (tableNameAttribute == null)
                throw new ArgumentNullException($"{type.FullName} does not have the {typeof(TableNameAttribute).FullName} attribute attached to it.");
            else if (string.IsNullOrWhiteSpace(tableNameAttribute.TableName))
                throw new ArgumentNullException($"{type.FullName} has the {typeof(TableNameAttribute).FullName} attribute attached to it, but the {nameof(TableNameAttribute.TableName)} property is blank.");

            PropertyInfo idProperty = type.GetProperties().FirstOrDefault(prop => Attribute.IsDefined(prop, typeof(IdAttribute)));
            if (idProperty == null)
                throw new ArgumentNullException($"{type.FullName} does not have a property with the {typeof(IdAttribute).FullName} attribute.");
            return Tuple.Create(tableNameAttribute, idProperty);
        }

        public static Dictionary<Type, string> GetMetaDataNames<TModel>() =>
            GetMetaDataNames(typeof(TModel));

        public static Dictionary<Type, string> GetMetaDataNames(Type type) {

            Tuple<TableNameAttribute, PropertyInfo> primaryInfoTuple =
                GetTableNameAndIdTuple(type);

            var dictionary = new Dictionary<Type, string> {
                { primaryInfoTuple.Item1.GetType(), primaryInfoTuple.Item1.TableName },
                { typeof(IdAttribute), primaryInfoTuple.Item2.Name },
            };

            /*
            Item1 => Type of property being searched for.
            Item2 => Type of attribute searching for.
            Item3 => Whether null is allowed for property.
            */

            new List<Tuple<Type, Type, bool>> {
                Tuple.Create(typeof(string), typeof(CreatorAttribute), false),
                Tuple.Create(typeof(string), typeof(UpdatorAttribute), true),
                Tuple.Create(typeof(DateTime?), typeof(DateCreatedAttribute), false),
                Tuple.Create(typeof(DateTime?), typeof(DateUpdatedAttribute), true),
            }.ForEach(tuple => {
                PropertyInfo property =
                    type
                        .GetProperties()
                        .Where(prop => prop.PropertyType == tuple.Item1)
                        .FirstOrDefault(prop => Attribute.IsDefined(prop, tuple.Item2));

                if (property == null) {
                    if (tuple.Item3)
                        return;
                    throw new ArgumentNullException($"{type.FullName} does not have a property with the {tuple.Item2.FullName} attribute.");
                }
                dictionary[tuple.Item2] = property.Name;
            });

            return dictionary;
        }
    }
}
