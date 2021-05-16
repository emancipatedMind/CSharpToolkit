namespace CSharpToolkit.Login.Abstractions {
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using CSharpToolkit.Utilities;
    public interface ILoginAttempter<TUser, TRole, TPassword> {
        Task<OperationResult<Tuple<TUser, List<TRole>>>> AttemptLoginAsync(string domain, string userName, TPassword password);
    }
}
