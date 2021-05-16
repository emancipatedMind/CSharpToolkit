namespace CSharpToolkit.DataAccess.Abstractions {
    using CSharpToolkit.DataAccess.Abstractions;
    public interface ISimpleAdminQueryProvider<TModel> : IAdminQueryProvider<TModel> {
        IAliasedCommandTypeDataOrder Lookup(TModel model);
    }
}
