namespace CSharpToolkit.Utilities.Abstractions {
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    public interface IExtendedNavigator<TItem> : INavigation<TItem>, IDisposable {
        TItem CurrentItem { get; }
        IReadOnlyList<TItem> NavigationCollection { get; }
        int NavigationIndex { get; }
        Task UpdateCurrentItem(TItem item);
        Func<TItem, Task> CurrentItemChangedCallback { get; set; }
        void UpdateNavigationCollection(IEnumerable<TItem> newCollection);
    }
}