namespace CSharpToolkit.Login.Abstractions {
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CSharpToolkit.Utilities;
    public interface ILoginDataAccessor<TUser, TRole> {
        Task<OperationResult<Tuple<TUser, List<TRole>>>> AttemptLoginAsync(string userName, string password);
    }
}
