﻿namespace CSharpToolkit.Abstractions.Logging {
    /// <summary>
    /// Implemented by class whose filename may be swapped.
    /// </summary>
    public interface IFileNameSwappable {
        /// <summary>
        /// File name setter.
        /// </summary>
        string FileName { set; }
    }
}
