namespace CSharpToolkit.ViewModels.Abstractions {
    using System;
    using System.Collections.Generic;
    using CSharpToolkit.Utilities.Abstractions;
    using CSharpToolkit.Validation.Abstractions;
    using DataAccess.Abstractions;
    public class SimplifiedModifyableTemplatedFindViewModel<TSearchCriteria, TSearchCriteriaImplementation, TSearchResults> : ModifyableTemplatedFindViewModel<TSearchResults, TSearchCriteriaImplementation, TSearchResults, TSearchCriteria, TSearchResults> where TSearchResults : IIdProvider where TSearchCriteriaImplementation : TSearchCriteria {
        public SimplifiedModifyableTemplatedFindViewModel(ISimpleFindDataAccessor<TSearchCriteria, TSearchResults> dataAccessor, IValidate<string> validator, Func<TSearchCriteriaImplementation> dataCollectorInitializer) : base(dataAccessor, validator, dataCollectorInitializer) { }
        protected override TSearchCriteria DataCollectorConverter(TSearchCriteriaImplementation dataCollector) => dataCollector;
        protected override IEnumerable<TSearchResults> DataGridSourceConverter(IEnumerable<TSearchResults> source) => source;
    }
}
