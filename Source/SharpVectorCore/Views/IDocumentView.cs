namespace SharpVectors.Dom.Views
{
    /// <summary>
    /// The document view interface is implemented by document objects in DOM implementations supporting 
    /// DOM Views. It provides an attribute to retrieve the default view of a document.
    /// </summary>
    public interface IDocumentView
    {
        /// <summary>
        /// The default <see cref="IAbstractView"/> for this document, or null if none available
        /// </summary>
        /// <value>An instance of <see cref="IAbstractView"/> representing the default view.</value>
        IAbstractView DefaultView { get; }
    }
}
