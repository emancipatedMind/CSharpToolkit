namespace CSharpToolkit.DataAccess.Abstractions {
    using CSharpToolkit.DataAccess.Abstractions;
    public interface ISimpleFindQueryProvider<TModel> {
        IAliasedCommandTypeDataOrder GetSearchQuery(TModel criteria);
    }
}
