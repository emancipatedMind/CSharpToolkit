namespace CSharpToolkit.Utilities.Abstractions {
    using System.Threading.Tasks;
    /// <summary>
    /// Adorned by an object that can participate in navigation.
    /// </summary>
    /// <typeparam name="T">The type being navigated.</typeparam>
    public interface INavigation<T> {
        /// <summary>
        /// Navigate forward.
        /// </summary>
        /// <returns>The result of the forward navigation.</returns>
        Task<OperationResult<T>> NavigateForward();
        /// <summary>
        /// Navigate backward.
        /// </summary>
        /// <returns>The result of the backward navigation.</returns>
        Task<OperationResult<T>> NavigateBack();
        /// <summary>
        /// Whether navigation forward is allowed.
        /// </summary>
        /// <returns>True represents that the navigation is allowed. False represents that the navigation is not allowed.</returns>
        bool CanNavigateForward();
        /// <summary>
        /// Whether navigation backward is allowed.
        /// </summary>
        /// <returns>True represents that the navigation is allowed. False represents that the navigation is not allowed.</returns>
        bool CanNavigateBack();
    }
}