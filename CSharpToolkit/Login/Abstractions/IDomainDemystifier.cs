namespace CSharpToolkit.Login.Abstractions {
    using System.Threading.Tasks;
    using CSharpToolkit.Utilities;
    public interface IDomainDemystifier<TUser, TRole> {
        Task<OperationResult<ISecureLoginDataAccessor<TUser, TRole>>> GetDataAccessor(string domain);
    }

}
