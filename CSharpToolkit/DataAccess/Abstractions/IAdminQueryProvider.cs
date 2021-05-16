namespace CSharpToolkit.DataAccess.Abstractions {
    using CSharpToolkit.DataAccess.Abstractions;
    public interface IAdminQueryProvider<TModel> {
        IAliasedCommandTypeDataOrder Create(TModel model);
        IAliasedCommandTypeDataOrder Create();
        IAliasedCommandTypeDataOrder Delete(TModel model);
        IAliasedCommandTypeDataOrder Update(TModel model);
    }
}
