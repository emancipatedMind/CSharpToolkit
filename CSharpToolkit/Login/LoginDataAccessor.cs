namespace CSharpToolkit.Login {
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using System.Security;
    using System.Threading.Tasks;
    using Abstractions;
    using DataAccess.Abstractions;
    using Extensions;
    using Utilities;
    using Utilities.Abstractions;
    using Validation;

    public class LoginDataAccessor<TUser, TRole, TUserImplementation, TRoleImplementation> : ISecureLoginDataAccessor<TUser, TRole> where TUserImplementation : class, TUser where TRoleImplementation : class, TRole {

        IDataAccessor _dataAccessor;
        ILoginQueryProvider _queryProvider;

        public LoginDataAccessor(IDataAccessor dataAccessor, ILoginQueryProvider queryProvider) {
            _dataAccessor = dataAccessor;
            _queryProvider = queryProvider;
        }

        public Task<OperationResult<Tuple<TUser, List<TRole>>>> AttemptLoginAsync(string userName, SecureString password) {
            OperationResult<string> passwordDecode = Decode.SecureString(password);
            return passwordDecode.HadErrors ?
                Task.FromResult(new OperationResult<Tuple<TUser, List<TRole>>>(passwordDecode.Exceptions)) :
                AttemptLoginAsync(userName, passwordDecode.Result);
        }

        public async Task<OperationResult<Tuple<TUser, List<TRole>>>> AttemptLoginAsync(string userName, string password) {
            var dataOrder = _queryProvider.AttemptLoginQuery(userName, password);

            OperationResult<List<DataRow>> operation =
                await _dataAccessor.SubmitQueryWithDataOrderAsync(dataOrder);
            if (operation.WasSuccessful == false) {
                return new OperationResult<Tuple<TUser, List<TRole>>>(operation.Exceptions);
            } 

            List<DataRow> rows = operation.Result;
            if (rows.Any() == false) {
                return new OperationResult<Tuple<TUser, List<TRole>>>(new Exception[] { new ValidationFailedException("Authentication has failed.") });
            }

            System.Diagnostics.Debug.Assert(rows.First()[_queryProvider.UserIdParameterName] is int,
                    $"{nameof(_queryProvider.UserIdParameterName)} must be integer."
                );

            int id = (int)rows.First()[_queryProvider.UserIdParameterName];

            var roles =
                await _dataAccessor.GetTableAsync<TRole, TRoleImplementation>(_queryProvider.GetRoleQuery(id));

            if (roles.WasSuccessful == false) {
                return new OperationResult<Tuple<TUser, List<TRole>>>(roles.Exceptions);
            }

            return new OperationResult<Tuple<TUser, List<TRole>>>(Tuple.Create<TUser, List<TRole>>(ProduceImplementation(rows, dataOrder.Aliases), roles.Result));
        }

        public Task<OperationResult<List<TRole>>> GetRoles(int userId) =>
            _dataAccessor.GetTableAsync<TRole, TRoleImplementation>(_queryProvider.GetRoleQuery(userId));

        #region Static Members
        static ConstructorInfo _userConstructor;
        static int _userConstructorParameterCount;
        static LoginDataAccessor() {
            Type userImplementationType =
                typeof(TUserImplementation);

            Type[][] types = new Type[][] {
                new Type[] { typeof(DataRow), typeof(IEnumerable<IAlias>) },
                new Type[] { typeof(DataRow) },
            };

            foreach(var parameterTypes in types) {
                _userConstructor = userImplementationType.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.HasThis, parameterTypes, null);
                if (_userConstructor != null) {
                    _userConstructorParameterCount = parameterTypes.Length;
                    break;
                }
            }

            System.Diagnostics.Debug.Assert(_userConstructor != null,
                    $"{userImplementationType.AssemblyQualifiedName} must contain a constructor that accepts {typeof(DataRow).AssemblyQualifiedName}"
                    + $" or contain a constructor that accepts {typeof(DataRow).AssemblyQualifiedName}, and {typeof(IEnumerable<IAlias>).AssemblyQualifiedName}."
                );
        }

        private static TUserImplementation ProduceImplementation(IEnumerable<DataRow> rows, IEnumerable<IAlias> aliases) {
            object[] parameters = _userConstructorParameterCount == 2 ? new object[] { rows.First(), aliases } : new object[] { rows.First() };
            return (TUserImplementation)_userConstructor.Invoke(parameters);
        }

        #endregion

    }
}
