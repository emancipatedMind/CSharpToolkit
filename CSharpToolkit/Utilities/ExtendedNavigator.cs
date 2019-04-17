namespace CSharpToolkit.Utilities {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Abstractions;
    using EventArgs;

    public class ExtendedNavigator<TItem> : IExtendedNavigator<TItem> {

        readonly List<TItem> _navigationCollection = new List<TItem>();
        bool disposedValue = false;
        TItem _currentItem;
        Func<TItem, Task> _currentItemChanged;

        public int NavigationIndex { get; private set; }
        public IReadOnlyList<TItem> NavigationCollection => _navigationCollection;
        public TItem CurrentItem => _currentItem;

        public void UpdateNavigationCollection(IEnumerable<TItem> newCollection) {
            _navigationCollection.Clear();
            _navigationCollection.AddRange(newCollection);
            NavigationIndex = _navigationCollection.IndexOf(CurrentItem);
        }

        public async Task<OperationResult<TItem>> NavigateForward() {
            bool navigationCanPerform = CanNavigateForward();
            if (navigationCanPerform) {
                await UpdateCurrentItem(_navigationCollection.ElementAt(++NavigationIndex));
            }
            return new OperationResult<TItem>(navigationCanPerform, CurrentItem);
        }

        public async Task<OperationResult<TItem>> NavigateBack() {
            bool navigationCanPerform = CanNavigateBack();
            if (navigationCanPerform) {
                await UpdateCurrentItem(_navigationCollection.ElementAt(--NavigationIndex));
            }
            return new OperationResult<TItem>(navigationCanPerform, CurrentItem);
        }

        public bool CanNavigateForward() => (
            NavigationCollection.Any() == false
            || NavigationIndex == -1
            || NavigationIndex + 1 == _navigationCollection.Count
        ) == false;

        public bool CanNavigateBack() => (
            NavigationCollection.Any() == false
            || NavigationIndex == -1
            || NavigationIndex == 0
        ) == false;

        public async Task UpdateCurrentItem(TItem item) {
            if (Perform.ReplaceIfDifferent(ref _currentItem, item).WasSuccessful) {
                NavigationIndex = _navigationCollection.IndexOf(_currentItem);
                await CurrentItemChangedCallback(_currentItem);
            }
        }

        public Func<TItem, Task> CurrentItemChangedCallback {
            get { return _currentItemChanged ?? (item => Task.CompletedTask); }
            set { _currentItemChanged = value; }
        }

        #region IDisposable Support
        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    CurrentItemChangedCallback = null;
                    _navigationCollection.Clear();
                }
                disposedValue = true;
            }
        }

        public void Dispose() => Dispose(true);
        #endregion

    }
}