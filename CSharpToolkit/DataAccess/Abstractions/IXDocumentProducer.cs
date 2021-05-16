namespace CSharpToolkit.DataAccess.Abstractions {
    /// <summary>
    /// Adorned by class who produces XDocument from an argument.
    /// </summary>
    /// <typeparam name="TArgument">The type that is accepted to produce the document.</typeparam>
    public interface IXDocumentProducer<TArgument> {
        /// <summary>
        /// Used to produce a document when given an argument.
        /// </summary>
        /// <param name="argument">The argument for use in producing the document.</param>
        /// <returns>The produced document from the argument.</returns>
        System.Xml.Linq.XDocument ProduceDocument(TArgument argument);
    }
}