namespace CSharpToolkit.Utilities.Abstractions {
    using DataAccess.Abstractions;
    /// <summary>
    /// Adorned by a class who may format a class adorned with <see cref="IAliasedCommandTypeDataOrder"/> .
    /// </summary>
    public interface IAliasedCommandTypeDataOrderFormatter {
        /// <summary>
        /// Formats the <see cref="IAliasedCommandTypeDataOrder"/>.
        /// </summary>
        /// <param name="order">The <see cref="IAliasedCommandTypeDataOrder"/> to format.</param>
        /// <returns>The formatted string.</returns>
        string Format(IAliasedCommandTypeDataOrder order);
    }
}