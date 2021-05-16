namespace CSharpToolkit.Login {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Abstractions;
    using CSharpToolkit.Extensions;
    using CSharpToolkit.Utilities;
    using CSharpToolkit.Validation;
    using CSharpToolkit.Logging.Abstractions;

    public class OpenDomainLoginAppInteractor<TUser, TRole> : ILoginAttempter<TUser, TRole, SecureString>, ILoginAttempter<TUser, TRole, string> {

        IDomainDemystifier<TUser, TRole> _dataAccessorAcquirer;

        public OpenDomainLoginAppInteractor(IDomainDemystifier<TUser, TRole> dataAccessorAcquirer, IExceptionLogger logger) {
            _dataAccessorAcquirer = dataAccessorAcquirer;
            ErrorLogger = logger;
        }

        public IExceptionLogger ErrorLogger { get; }

        public Task<OperationResult<Tuple<TUser, List<TRole>>>> AttemptLoginAsync(string domain, string userName, string password) =>
            AttemptLoginAsync(domain, password, (dA, pw) => dA.AttemptLoginAsync(userName, password));

        public Task<OperationResult<Tuple<TUser, List<TRole>>>> AttemptLoginAsync(string domain, string userName, SecureString password) =>
            AttemptLoginAsync(domain, password, (dA, pw) => dA.AttemptLoginAsync(userName, password));

        async Task<OperationResult<Tuple<TUser, List<TRole>>>> AttemptLoginAsync<TPassword>(string domain, TPassword password, Func<ISecureLoginDataAccessor<TUser, TRole>, TPassword, Task<OperationResult<Tuple<TUser, List<TRole>>>>> callback) {
            if (domain.IsMeaningful() == false)
                return new OperationResult<Tuple<TUser, List<TRole>>>(new[] { new ValidationFailedException("Invalid Domain.") });

            var dataAccessorOperation =
                await _dataAccessorAcquirer.GetDataAccessor(domain);

            if (dataAccessorOperation.WasSuccessful == false) {
                if (dataAccessorOperation.HadErrors) {
                    await ErrorLogger.LogExceptionsAsync(dataAccessorOperation.Exceptions);
                    return new OperationResult<Tuple<TUser, List<TRole>>>(dataAccessorOperation.Exceptions);
                }
                return new OperationResult<Tuple<TUser, List<TRole>>>(new[] { new ValidationFailedException("Authentication has failed.") });
            }

            var loginAttemptOperation =
                await callback(dataAccessorOperation.Result, password);

            if (loginAttemptOperation.WasSuccessful == false) {
                var nonValidationExceptions =
                    loginAttemptOperation.Exceptions.Where(e => e is ValidationFailedException == false);
                if (nonValidationExceptions.Any()) {
                    await ErrorLogger.LogExceptionsAsync(nonValidationExceptions.ToArray());
                }
            }
            return loginAttemptOperation;
        }
    }
}
