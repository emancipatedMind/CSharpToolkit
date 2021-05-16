namespace CSharpToolkit.Extensions {
    using Utilities.Abstractions;
    using ViewModels.Abstractions;
    using CSharpToolkit.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    public static class ViewModelExtensions {
        public static void ImportLockers(this WindowViewModel vm, IParentViewModel parentViewModel) {
            vm.ApplicationRestrictor = parentViewModel?.GetFormRestrictor();
            vm.FamilialLocker = parentViewModel?.GetFamilialLocker();
        }

        public static bool StateIdleAndAnyItemsSelectedEnable<TViewModel, TLocator>(this TViewModel vm, TLocator locator) where TViewModel : WindowViewModel where TLocator : ILocator =>
            vm.StateIdleEnable() && locator.AnyItemsSelectedEnable();

        public static bool StateNotCriticalEnable<TViewModel>(this TViewModel vm) where TViewModel : WindowViewModel => (
            vm.State == ViewModelState.CriticalOperation
            || vm.State == ViewModelState.NonCriticalOperation
        ) == false;

        public static bool StateNotRestrictedEnable<TViewModel>(this TViewModel vm) where TViewModel : WindowViewModel =>
            vm.State != ViewModelState.Restricted;

        /* --------------------------------*/

        public static bool StateCriticalOperationEnable<TViewModel>(this TViewModel vm) where TViewModel : WindowViewModel =>
            vm.State == ViewModelState.CriticalOperation;

        public static bool StateNotFamilialOperationEnable<TViewModel>(this TViewModel vm) where TViewModel : WindowViewModel =>
            vm.State != ViewModelState.FamilialOperation;

        public static bool StateFreeEnable<TViewModel>(this TViewModel vm) where TViewModel : WindowViewModel => (
            vm.State == ViewModelState.CriticalOperation
            || vm.State == ViewModelState.NonCriticalOperation
            || vm.State == ViewModelState.FamilialOperation
        ) == false;

        public static bool StateIdleEnable<TViewModel>(this TViewModel vm) where TViewModel : WindowViewModel =>
            vm.State == ViewModelState.Idle;

    }
}