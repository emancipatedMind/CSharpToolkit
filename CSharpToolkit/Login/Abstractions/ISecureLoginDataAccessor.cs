namespace CSharpToolkit.Login.Abstractions {
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using CSharpToolkit.Utilities;
    public interface ISecureLoginDataAccessor<TUser, TRole> : ILoginDataAccessor<TUser, TRole> {
        Task<OperationResult<Tuple<TUser, List<TRole>>>> AttemptLoginAsync(string userName, SecureString password);
    }
}
