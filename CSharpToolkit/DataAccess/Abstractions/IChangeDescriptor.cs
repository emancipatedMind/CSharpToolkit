namespace CSharpToolkit.DataAccess.Abstractions {
    using System.Collections.Generic;
    using System;
    /// <summary>
    /// Implemented by a class to keep a running list of properties that have been changed.
    /// </summary>
    public interface IChangeDescriptor {
        /// <summary>
        /// A list of property names that have been changed along with current value.
        /// </summary>
        List<PropertyModification> GetChangedProperties();
    }
}