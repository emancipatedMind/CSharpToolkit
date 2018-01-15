namespace CSharpToolkit.DataAccess {
    using Abstractions;
    using System.Collections.Generic;
    using Utilities.Abstractions;
    public class AliasedDataOrder : CSharpToolkit.DataAccess.SimpleDataOrder, IAliasedDataOrder {

        public AliasedDataOrder(ISimpleDataOrder order) : base(order.Query, order.Parameters) { }
        public AliasedDataOrder(string query, IEnumerable<KeyValuePair<string, object>> parameters) : base(query, parameters) { }
        public IEnumerable<IAlias> Aliases { get; set; }

    }
}