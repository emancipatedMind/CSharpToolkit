namespace CSharpToolkit.Utilities.Abstractions {
    using System;
    using System.Threading.Tasks;
    /// <summary>
    /// Implemented by a class who requires set up.
    /// </summary>
    public interface IInitializable : IDisposable {
        /// <summary>
        /// Reset the instance.
        /// </summary>
        Task Reset();
        /// <summary>
        /// Initialize instance.
        /// </summary>
        Task Initialize();
    }
}