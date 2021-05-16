namespace CSharpToolkit.DataAccess {
    using Abstractions;
    using System;
    using System.Collections.Generic;
    using Utilities;
    using Extensions;
    using System.Reflection;
    using System.Threading.Tasks;
    [Obsolete("Included to not break current code, but should be avoided. Use class which uses three types instead of two.")]
    public class SimpleFindDataAccessor<TOrder, TDataGridSource> : SimpleFindDataAccessor<TOrder, TDataGridSource, TDataGridSource> {
        public SimpleFindDataAccessor(IDataAccessor dataAccessor, ISimpleFindQueryProvider<TOrder> queryProvider) : base(dataAccessor, queryProvider) { }
    }

    public class SimpleFindDataAccessor<TOrder, TDataGridSource, TConcreteDataGridSource> : ISimpleFindDataAccessor<TOrder, TDataGridSource> where TConcreteDataGridSource : TDataGridSource {

        static ConstructorInfo _constructor;
        static SimpleFindDataAccessor() {
            Type dataGridType = typeof(TConcreteDataGridSource);
            _constructor = dataGridType.GetConstructor(new Type[] { typeof(System.Data.DataRow) });
            System.Diagnostics.Debug.Assert(_constructor != null, $"{dataGridType.AssemblyQualifiedName} does not have an eligible constructor accepting {typeof(System.Data.DataRow).AssemblyQualifiedName}.");
        }

        ISimpleFindQueryProvider<TOrder> _queryProvider;
        IDataAccessor _dataAccessor;

        public SimpleFindDataAccessor(IDataAccessor dataAccessor, ISimpleFindQueryProvider<TOrder> queryProvider) {
            _dataAccessor = dataAccessor;
            _queryProvider = queryProvider;
        }

        public virtual Task<OperationResult<List<TDataGridSource>>> SubmitSearchOrderAsync(TOrder dataCollector) =>
            _dataAccessor.GetTableAsync(_queryProvider.GetSearchQuery(dataCollector), row => (TDataGridSource)_constructor.Invoke(new[] { row }), GetType, nameof(SubmitSearchOrderAsync));

    }
}
