namespace CSharpToolkit.EventArgs {
    using System;
    using System.Collections.Generic;
    public class GenericCollectionEventArgs<T> : EventArgs {
        public GenericCollectionEventArgs(IEnumerable<T> collection) {
            Data = new List<T>(collection);
        }
        public List<T> Data { get; private set; }
    }
}
