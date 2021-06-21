namespace SharpVectors.Dom.Views
{
    /// <summary>
    /// A base interface that all views shall derive from
    /// </summary>
    public interface IAbstractView
    {
        /// <summary>
        /// The source <see cref="IDocumentView"/> of which this is an <see cref="IAbstractView"/>.
        /// </summary>
        /// <value>An instance of <see cref="IDocumentView"/> representing the document view.</value>
        IDocumentView Document { get; }
    }
}
