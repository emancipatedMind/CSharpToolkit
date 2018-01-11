namespace CSharpToolkit.EventArgs {
    /// <summary>
    /// Generic event args for general purposes.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    public class GenericEventArgs<T> : System.EventArgs {
        /// <summary>
        /// Data provided by event.
        /// </summary>
        public T Data { get; private set; }
        /// <summary>
        /// Instantiates GenericEventArgs with data.
        /// </summary>
        /// <param name="data">Data for listeners.</param>
        public GenericEventArgs(T data) {
            Data = data;
        }
    }
    /// <summary>
    /// Generic event args for general purposes.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <typeparam name="T2">SecondaryData type.</typeparam>
    public class GenericEventArgs<T, T2> : System.EventArgs {
        /// <summary>
        /// Data provided by event.
        /// </summary>
        public T Data { get; private set; }
        /// <summary>
        /// Secondary data provided by event.
        /// </summary>
        public T2 SecondaryData { get; private set; }
        /// <summary>
        /// Instantiates GenericEventArgs with data.
        /// </summary>
        /// <param name="data">Data for listeners.</param>
        /// <param name="secondaryData">Secondary data for listeners.</param>
        public GenericEventArgs(T data, T2 secondaryData) {
            Data = data;
            SecondaryData = secondaryData;
        }
    }
}