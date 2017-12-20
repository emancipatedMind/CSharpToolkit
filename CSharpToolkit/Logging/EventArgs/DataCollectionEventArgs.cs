namespace CSharpToolkit.EventArgs {
    using System;
    public class DataCollectionEventArgs<T> : EventArgs {
        public T DataCollected { get; set; }
    }
}
