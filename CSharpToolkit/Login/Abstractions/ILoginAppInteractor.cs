namespace CSharpToolkit.Login.Abstractions {
    using System.Security;
    public interface ILoginAppInteractor<TUser, TRole, TPassword> : IDomainProvider, ILoginAttempter<TUser, TRole, TPassword> {
        bool LoginAllowed(string userName, SecureString _password);
    }
}
