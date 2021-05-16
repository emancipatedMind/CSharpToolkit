namespace CSharpToolkit.DataAccess {
    using System;
    using Abstractions;
    using CSharpToolkit.DataAccess.Abstractions;
    /// <summary>
    /// A simple class that can apply a function to search criteria before passing to sql.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class SearchCriteriaModifier<TModel> : ISimpleFindQueryProvider<TModel> {

        ISimpleFindQueryProvider<TModel> _component;
        Func<TModel, TModel> _modifierFunction;

        public SearchCriteriaModifier(ISimpleFindQueryProvider<TModel> component, Func<TModel, TModel> modifierFunction) {
            _component = component;
            _modifierFunction = modifierFunction;
        }

        public IAliasedCommandTypeDataOrder GetSearchQuery(TModel criteria) => _component.GetSearchQuery(_modifierFunction(criteria));
    }
}
