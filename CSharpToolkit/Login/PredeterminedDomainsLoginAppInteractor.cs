namespace CSharpToolkit.Login {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Abstractions;
    using CSharpToolkit.DataAccess.Abstractions;
    using CSharpToolkit.Extensions;
    using CSharpToolkit.Logging.Abstractions;
    using CSharpToolkit.Utilities;
    using CSharpToolkit.Utilities.EventArgs;
    using CSharpToolkit.Validation;

    public class PredeterminedDomainsLoginAppInteractor<TUser, TRole> : ILoginAppInteractor<TUser, TRole, string>, ILoginAppInteractor<TUser, TRole, SecureString> {

        Dictionary<string, ISecureLoginDataAccessor<TUser, TRole>> _domains;

        public PredeterminedDomainsLoginAppInteractor(Dictionary<string, ISecureLoginDataAccessor<TUser, TRole>> domains, IExceptionLogger logger) {
            _domains = domains ?? new Dictionary<string, ISecureLoginDataAccessor<TUser, TRole>>();
            ErrorLogger = logger;
        }

        public Task<OperationResult<Tuple<TUser, List<TRole>>>> AttemptLoginAsync(string domain, string userName, string password) =>
            AttemptLoginAsync(domain, password, (dA, pw) => dA.AttemptLoginAsync(userName, password));

        public Task<OperationResult<Tuple<TUser, List<TRole>>>> AttemptLoginAsync(string domain, string userName, SecureString password) =>
            AttemptLoginAsync(domain, password, (dA, pw) => dA.AttemptLoginAsync(userName, password));

        async Task<OperationResult<Tuple<TUser, List<TRole>>>> AttemptLoginAsync<TPassword>(string domain, TPassword password, Func<ISecureLoginDataAccessor<TUser, TRole>, TPassword, Task<OperationResult<Tuple<TUser, List<TRole>>>>> callback) {
            ISecureLoginDataAccessor<TUser, TRole> dataAccessor;
            if (_domains.ContainsKey(domain))
                dataAccessor = _domains[domain];
            else 
                return new OperationResult<Tuple<TUser, List<TRole>>>(new[] { new ValidationFailedException("Invalid Domain.") });

            var loginAttemptOperation =
                await callback(dataAccessor, password);

            if (loginAttemptOperation.WasSuccessful == false) {
                var nonValidationExceptions =
                    loginAttemptOperation.Exceptions.Where(e => e is ValidationFailedException == false);
                if (nonValidationExceptions.Any()) {
                    await ErrorLogger.LogExceptionsAsync(nonValidationExceptions.ToArray());
                }
            }
            return loginAttemptOperation;
        }

        public Task<OperationResult<string>> GetDefaultDomainsAsync() =>
            Task.FromResult(new OperationResult<string>(_domains.FirstOrDefault().Key ?? ""));

        public Task<OperationResult<string[]>> GetDomainsAsync() =>
            Task.FromResult(new OperationResult<string[]>(_domains.Select(item => item.Key).ToArray()));

        public bool LoginAllowed(string userName, SecureString _password) =>
            userName.IsMeaningful() || _password.Length > 1;

        public IExceptionLogger ErrorLogger { get; }

    }
}
