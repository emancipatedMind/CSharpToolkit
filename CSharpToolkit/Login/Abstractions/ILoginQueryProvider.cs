namespace CSharpToolkit.Login.Abstractions {
    using CSharpToolkit.DataAccess.Abstractions;
    public interface ILoginQueryProvider {
        IAliasedCommandTypeDataOrder AttemptLoginQuery(string userName, string password);
        IAliasedCommandTypeDataOrder GetRoleQuery(int userId);
        string UserIdParameterName { get; }
    }
}
