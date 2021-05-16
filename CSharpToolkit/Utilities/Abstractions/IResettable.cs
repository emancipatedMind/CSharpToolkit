using System.Threading.Tasks;

namespace CSharpToolkit.Utilities.Abstractions {
    /// <summary>
    /// Adorned by a class who is resettable.
    /// </summary>
    public interface IResettable {
        /// <summary>
        /// Reset the instance.
        /// </summary>
        /// <returns>Whether the reset was successful or not.</returns>
        Task<OperationResult> Reset();
    }
}
