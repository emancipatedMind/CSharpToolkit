namespace CSharpToolkit.XAML {
    using System;
    using System.Collections.Generic;
    using System.Collections;
    using CSharpToolkit.EventArgs;

    public class CustomCollection<T> : IEnumerable<T> {
        public event EventHandler<GenericCollectionEventArgs<T>> CollectionAddedTo;
        public event EventHandler<GenericCollectionEventArgs<T>> CollectionCleared;

        List<T> _collection = new List<T>();

        public void AddRange(IEnumerable<T> range) {
            _collection.AddRange(range);
            CollectionAddedTo?.Invoke(this, new GenericCollectionEventArgs<T>(range));
        }

        public void Clear() {
            var clone = new List<T>(_collection);
            _collection.Clear();
            CollectionCleared?.Invoke(this, new GenericCollectionEventArgs<T>(clone));
        }

        public int Count => _collection.Count;

        public void ForEach(Action<T> action) => _collection.ForEach(action);

        public IEnumerator<T> GetEnumerator() =>
            _collection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            _collection.GetEnumerator();
    }
}
