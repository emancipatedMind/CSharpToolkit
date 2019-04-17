namespace CSharpToolkit.XAML.Behaviors {
    /// <summary>
    /// An implementation that does nothing in the conversions.
    /// </summary>
    public class NullListItemConverter : Abstractions.IListItemConverter {
        /// <summary>
        /// Converts the specified master list item.
        /// </summary>
        /// <param name="masterListItem">The master list item.</param>
        /// <returns>The result of the conversion</returns>
        public object Convert(object masterListItem) => masterListItem;
        /// <summary>
        /// Converts the specified target list item.
        /// </summary>
        /// <param name="targetListItem">The target list item.</param>
        /// <returns>The result of the conversion</returns>
        public object ConvertBack(object targetListItem) => targetListItem;
    }
}