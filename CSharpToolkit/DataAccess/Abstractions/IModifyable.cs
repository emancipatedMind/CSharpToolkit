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
        /// Reset the data using a name.
        /// </summary>
        void Reset(string name);
        /// <summary>
        /// Whether any changes exist since last Save/Reset.
        /// </summary>
        bool Modified { get; }
    }
}
