namespace CSharpToolkit.Utilities {
    using System.Collections;
    using System.Collections.Generic;
    public class GenericContainer<T> : IEnumerable {

        public List<T> ContainedItems { get; } = new List<T>();

        public IEnumerator GetEnumerator() =>
            ContainedItems.GetEnumerator();

        public void Add(T item) =>
            ContainedItems.Add(item);

    }
}
