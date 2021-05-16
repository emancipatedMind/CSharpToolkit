namespace CSharpToolkit.Extensions {
    using Utilities.Abstractions;
    using CSharpToolkit.Extensions;
    using CSharpToolkit.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public static class LocatorExtensions {

        public static void RequestEnable(this IEnumerable<ILocator> source, object token) =>
            source?.ForEach(loc => loc?.RequestEnable(token));
        public static void RequestDisable(this IEnumerable<ILocator> source, object token) =>
            source?.ForEach(loc => loc?.RequestDisable(token));

        public static bool SingleItemSelectedEnable(this ILocator locator) => (
                locator == null
                || locator.SelectedItems.GroupBy(x => x.Id).Count() != 1
            ) == false;

        public static bool AnyItemsSelectedEnable(this ILocator locator) => (
                locator == null
                || locator.SelectedItems.Any() == false
            ) == false;

        public static bool AnyItemsSelectedEnable<T>(this IVMLocator<T> locator, Func<T, bool> function) where T : IIdProvider => (
                locator == null
                || locator.GetSelectedItems().Any(function) == false
            ) == false;

        public static bool ItemsFoundEnable(this ILocator locator) => (
                locator == null
                || locator.CurrentItems.Any() == false
            ) == false;

        public static Task RefreshLocator<TLocator>(this TLocator locator, object token) where TLocator : ILocator =>
            RefreshLocator(locator, locator.Constraint, token);

        public static async Task RefreshLocator<TLocator>(this TLocator locator, int constraint, object token) where TLocator : ILocator {
            if (locator == null)
                return;

            locator.ClearAndUpdateConstraint(constraint, token);
            if (constraint == 0)
                return;
            await locator.RefreshCollections();
            await locator.RequestSearch();
            locator.RequestEnable(token);
        }

        public static void ClearAndUpdateConstraint<TLocator>(this TLocator locator, int constraint, object token) where TLocator : ILocator {
            if (locator == null)
                return;

            locator.ClearAndDisableLocator(token);
            locator.Constraint = constraint;
        }

        public static void ClearAndDisableLocator<TLocator>(this TLocator locator, object token) where TLocator : ILocator {
            locator?.RequestDisable(token);
            locator?.RequestClear();
        }

        public static Task<OperationResult> LockLocatorAndRunMethodWithLocator<TLocator>(this TLocator locator, Func<TLocator, Task<OperationResult>> method, bool refreshLocatorUponSuccess = true) where TLocator : ILocator =>
            LockLocatorAndRunMethod(locator, () => method(locator), new object(), refreshLocatorUponSuccess);

        public static Task<OperationResult> LockLocatorAndRunMethodWithLocator<TLocator>(this TLocator locator, Func<TLocator, Task<OperationResult>> method, object token, bool refreshLocatorUponSuccess = true) where TLocator : ILocator =>
            LockLocatorAndRunMethod(locator, () => method(locator), token, refreshLocatorUponSuccess);

        public static Task<OperationResult> LockLocatorAndRunMethodWithLocator<TLocator>(this TLocator locator, Func<Task<OperationResult>> method, bool refreshLocatorUponSuccess = true) where TLocator : ILocator =>
            LockLocatorAndRunMethod(locator, () => method(), new object(), refreshLocatorUponSuccess);

        public static async Task<OperationResult> LockLocatorAndRunMethod<TLocator>(this TLocator locator, Func<Task<OperationResult>> method, object token, bool refreshLocatorUponSuccess = true) where TLocator : ILocator {
            if (locator == null)
                return new OperationResult(false);

            locator.RequestDisable(token);
            OperationResult operation =
                await method();
            if (operation.WasSuccessful && refreshLocatorUponSuccess)
                await locator.RefreshLocator(token);
            locator.RequestEnable(token);
            return operation;
        }

        public static Task LockLocatorAndRunMethodWithLocator<TLocator>(this TLocator locator, Func<TLocator, Task> method, bool refreshLocatorUponSuccess = true) where TLocator : ILocator =>
            LockLocatorAndRunMethod(locator, () => method(locator), new object(), refreshLocatorUponSuccess);

        public static Task LockLocatorAndRunMethodWithLocator<TLocator>(this TLocator locator, Func<TLocator, Task> method, object token, bool refreshLocatorUponSuccess = true) where TLocator : ILocator =>
            LockLocatorAndRunMethod(locator, () => method(locator), token, refreshLocatorUponSuccess);

        public static Task LockLocatorAndRunMethod<TLocator>(this TLocator locator, Func<Task> method, bool refreshLocatorUponSuccess = true) where TLocator : ILocator =>
            LockLocatorAndRunMethod(locator, () => method(), new object(), refreshLocatorUponSuccess);

        public static async Task LockLocatorAndRunMethod<TLocator>(this TLocator locator, Func<Task> method, object token, bool refreshLocatorUponSuccess = true) where TLocator : ILocator {
            if (locator == null)
                return;

            locator.RequestDisable(token);
            await method();
            if (refreshLocatorUponSuccess)
                await locator.RefreshLocator(token);
            locator.RequestEnable(token);
        }

        public static IEnumerable<T> GetSelectedItems<T>(this IVMLocator<T> locator) where T : CSharpToolkit.Utilities.Abstractions.IIdProvider {
            if (locator == null)
                return Enumerable.Empty<T>();
            return (
                from s in locator.SelectedItems
                join m in locator.ModifyableItems on s.Id equals m.Id
                select m
            ).ToArray();
        }


    }
}