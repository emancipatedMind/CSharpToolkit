namespace CSharpToolkit.DataAccess {
    using System.Collections.Generic;
    using Abstractions;
    using CSharpToolkit.DataAccess.Abstractions;
    using CSharpToolkit.Utilities;
    using CSharpToolkit.Utilities.Abstractions;
    using System.Reflection;
    using CSharpToolkit.Extensions;
    using Extensions;
    using System.Threading.Tasks;
    public class SimpleAdminDataAccessor<TDataOrder, TModelAbstraction, TModelImplementation> : ISimpleAdminDataAccessor<TDataOrder, TModelAbstraction> where TModelImplementation : TModelAbstraction {

        static ConstructorInfo _constructorForType;
        static bool _useBackupConstructor;

        static SimpleAdminDataAccessor() {
            _constructorForType = typeof(TModelImplementation).GetConstructor(new[] { typeof(System.Data.DataRow), typeof(IEnumerable<IAlias>) });
            if (_constructorForType == null) {
                _constructorForType = typeof(TModelImplementation).GetConstructor(new[] { typeof(System.Data.DataRow) });
                _useBackupConstructor = true;
            }
            System.Diagnostics.Debug.Assert(_constructorForType != null, $"{typeof(TModelImplementation).AssemblyQualifiedName} does not contain the eligible constructor which expects {typeof(System.Data.DataRow).AssemblyQualifiedName} as its only parameter.");
        }

        ISimpleAdminQueryProvider<TDataOrder> _queryProvider;
        IDataAccessor _dataAccessor;

        public SimpleAdminDataAccessor(IDataAccessor dataAccessor, ISimpleAdminQueryProvider<TDataOrder> queryProvider) {
            _dataAccessor = dataAccessor;
            _queryProvider = queryProvider;
        }

        public Task<OperationResult<int>> CreateAsync(TDataOrder model) =>
            _dataAccessor.CreateAsync(_queryProvider.Create(model), GetType, nameof(CreateAsync));

        public Task<OperationResult<int>> CreateAsync() =>
            _dataAccessor.CreateAsync(_queryProvider.Create(), GetType, nameof(CreateAsync));

        public Task<OperationResult> DeleteAsync(TDataOrder model) =>
            _dataAccessor.DeleteAsync(_queryProvider.Delete(model), GetType, nameof(DeleteAsync));

        public Task<OperationResult> UpdateAsync(TDataOrder model) =>
            _dataAccessor.UpdateAsync(_queryProvider.Update(model), GetType, nameof(UpdateAsync));

        public Task<OperationResult<TModelAbstraction>> LookupAsync(TDataOrder model) =>
            _dataAccessor.LookupAsync<TModelAbstraction, TModelImplementation>(_queryProvider.Lookup(model), (dataOrder, row) => (TModelImplementation)(_useBackupConstructor ? _constructorForType.Invoke(new[] { row }) : _constructorForType.Invoke(new object[] { row, dataOrder.Aliases })), GetType, nameof(LookupAsync));

    }

    public class SimpleAdminDataAccessor<TModelAbstraction, TModelImplementation> : SimpleAdminDataAccessor<TModelAbstraction, TModelAbstraction, TModelImplementation>, ISimpleAdminDataAccessor<TModelAbstraction> where TModelImplementation : TModelAbstraction {
        public SimpleAdminDataAccessor(IDataAccessor dataAccessor) : this(dataAccessor, new SimpleAdminQueryProvider<TModelAbstraction>()) { }
        public SimpleAdminDataAccessor(IDataAccessor dataAccessor, ISimpleAdminQueryProvider<TModelAbstraction> queryProvider) : base(dataAccessor, queryProvider) { }
    }
}
