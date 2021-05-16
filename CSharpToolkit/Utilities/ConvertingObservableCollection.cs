namespace CSharpToolkit.Utilities {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;

    public class ConvertingObservableCollection<T, T2> : IList<T>, INotifyCollectionChanged, IDisposable where T2 : T {

        static Func<T, T2> DefaultConvert = new Func<T, T2>(item => (T2)item);

        bool clearList = true;
        bool disposedValue = false;
        ObservableCollection<T> _internalList;
        Func<T, T2> _convert;

        public ConvertingObservableCollection(Func<T, T2> convert) : this(Enumerable.Empty<T>(), convert) { }
        public ConvertingObservableCollection(IEnumerable<T> collection, Func<T, T2> convert) {
            _internalList = new ObservableCollection<T>(collection);
            _internalList.CollectionChanged += _internalList_CollectionChanged;
            _convert = convert ?? DefaultConvert;
        }
        public ConvertingObservableCollection(ObservableCollection<T> collection, Func<T, T2> convert) {
            _internalList = collection;
            _internalList.CollectionChanged += _internalList_CollectionChanged;
            _convert = convert ?? DefaultConvert;
            clearList = false;
        }

        public int Count => ((IList<T>)_internalList).Count;

        public bool IsReadOnly =>
            ((IList<T>)_internalList).IsReadOnly;

        public T this[int index] {
            get { return ((IList<T>)_internalList)[index]; }
            set { ((IList<T>)_internalList)[index] = _convert(value); }
        }

        public int IndexOf(T item) =>
            ((IList<T>)_internalList).IndexOf(item);

        public void Insert(int index, T item) =>
            ((IList<T>)_internalList).Insert(index, _convert(item));

        public void RemoveAt(int index) =>
            ((IList<T>)_internalList).RemoveAt(index);

        public void Add(T item) =>
            // Here, convert is used to convert item.
            ((IList<T>)_internalList).Add(_convert(item));

        public void Clear() =>
            ((IList<T>)_internalList).Clear();

        public bool Contains(T item) =>
            ((IList<T>)_internalList).Contains(item);

        public void CopyTo(T[] array, int arrayIndex) =>
            ((IList<T>)_internalList).CopyTo(array, arrayIndex);

        public bool Remove(T item) =>
            ((IList<T>)_internalList).Remove(item);

        public IEnumerator<T> GetEnumerator() =>
            ((IList<T>)_internalList).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            ((IList<T>)_internalList).GetEnumerator();

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void _internalList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) =>
            CollectionChanged?.Invoke(sender, e);

        #region IDisposable Support

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    _internalList.CollectionChanged -= _internalList_CollectionChanged;
                    if (clearList)
                        _internalList.Clear();
                    _internalList = null;
                    CollectionChanged = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose() => Dispose(true);
        #endregion

    }
}
