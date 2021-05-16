namespace CSharpToolkit.ViewModels.Abstractions {
    using System;
    using System.Collections.Generic;
    using CSharpToolkit.Utilities.Abstractions;
    using CSharpToolkit.Validation.Abstractions;
    using DataAccess.Abstractions;
    public class SimplifiedModifyableTemplatedFindViewModelWithInteractor<TSearchCriteriaImplementation, TSearchCriteria, TSearchResults> : ModifyableTemplatedFindViewModelWithInteractor<TSearchResults, TSearchCriteriaImplementation, TSearchResults, TSearchCriteria, TSearchResults> where TSearchResults : IIdProvider where TSearchCriteriaImplementation : TSearchCriteria {
        public SimplifiedModifyableTemplatedFindViewModelWithInteractor(ISimpleFindInteractor<TSearchCriteriaImplementation, TSearchCriteria, TSearchResults> interactor, IValidate<string> validator, Func<TSearchCriteriaImplementation> dataCollectorInitializer) : base(interactor, validator, dataCollectorInitializer) { }

        protected override TSearchCriteria DataCollectorConverter(TSearchCriteriaImplementation dataCollector) => dataCollector;

        protected override IEnumerable<TSearchResults> DataGridSourceConverter(IEnumerable<TSearchResults> source) => source;
    }
}
