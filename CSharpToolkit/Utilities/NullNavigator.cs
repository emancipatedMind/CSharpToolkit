namespace CSharpToolkit.Utilities {
    using System.Threading.Tasks;
    /// <summary>
    /// An implementation of the null pattern for the <see cref="Abstractions.INavigation{T}"/> interface.
    /// </summary>
    /// <typeparam name="T">The type of navigation.</typeparam>
    public class NullNavigator<T> : Abstractions.INavigation<T> {

        static NullNavigator<T> _instance;

        /// <summary>
        /// The singleton of the <see cref="NullNavigator{T}"/>.
        /// </summary>
        public static NullNavigator<T> Instance => _instance ?? (_instance = new NullNavigator<T>());

        private NullNavigator() { }

        /// <summary>
        /// Navigation is denied.
        /// </summary>
        /// <returns>False</returns>
        public bool CanNavigateBack() =>
            false;

        /// <summary>
        /// Navigation is denied.
        /// </summary>
        /// <returns>False</returns>
        public bool CanNavigateForward() =>
            false;

        /// <summary>
        /// Navigation is denied.
        /// </summary>
        /// <returns>An unsuccessful operation result, and the default of the type.</returns>
        public Task<OperationResult<T>> NavigateBack() =>
            Task.FromResult(new OperationResult<T>(false, default(T)));

        /// <summary>
        /// Navigation is denied.
        /// </summary>
        /// <returns>An unsuccessful operation result, and the default of the type.</returns>
        public Task<OperationResult<T>> NavigateForward() =>
            Task.FromResult(new OperationResult<T>(false, default(T)));

    }

}