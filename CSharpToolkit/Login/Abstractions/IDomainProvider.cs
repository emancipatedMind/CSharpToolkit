namespace CSharpToolkit.Login.Abstractions {
    using System.Threading.Tasks;
    using CSharpToolkit.Utilities;
    public interface IDomainProvider {
        Task<OperationResult<string[]>> GetDomainsAsync();
        Task<OperationResult<string>> GetDefaultDomainsAsync();
    }
}
