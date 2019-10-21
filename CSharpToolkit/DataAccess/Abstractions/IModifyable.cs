namespace CSharpToolkit.DataAccess.Abstractions {
    /// <summary>
    /// Implemented by a class who is modifyable allowing data to be saved, and reset.
    /// </summary>
    public interface IModifyable {
        /// <summary>
        /// Save the data.
        /// </summary>
        void Save(); 
        /// <summary>
        /// Reset the data.
        /// </summary>
        void Reset();
        /// <summary>
        /// Reset a specific property.
        /// </summary>
        /// <param name="name">Property name to change.</param>
        /// <returns>Whether property was reset.</returns>
        bool Reset(string name);
        /// <summary>
        /// Whether any changes exist since last Save/Reset.
        /// </summary>
        bool Modified { get; }
    }
}
