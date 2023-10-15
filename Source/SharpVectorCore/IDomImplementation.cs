namespace SharpVectors.Dom
{
    /// <summary>
    /// The <see cref="IDomImplementation"/> interface provides a number of methods for performing operations 
    /// that are independent of any particular instance of the document object model.
    /// </summary>
    /// <seealso href="http://www.w3.org/TR/2000/REC-DOM-Level-2-Core-20001113">
    /// Document Object Model (DOM) Level 2 Core Specification</seealso>
	public interface IDomImplementation
	{
        /// <summary>
        /// Test if the DOM implementation implements a specific feature. </summary>
        /// <param name="feature"> name of the feature to test (case-insensitive). The values used by DOM features 
        /// are defined throughout the DOM Level 2 specifications and listed in the  section. The name must be an XML
        /// name. To avoid possible conflicts, as a convention, names referring to features defined outside the DOM 
        /// specification should be made unique by reversing the name of the Internet domain name of the person 
        /// (or the organization that the person belongs to) who defines the feature, component by component, and using 
        /// this as a prefix. For instance, the W3C SVG Working Group defines the feature "org.w3c.dom.svg".
        /// </param>
        /// <param name="version"> is the version number of the feature to test. In Level 2, the string can be 
        /// either "2.0" or "1.0". If the version is not specified, supporting any version of the feature causes 
        /// the method to return <see langword="true"/>. </param>
        /// <returns> <see langword="true"/> if the feature is implemented in the specified version, 
        /// <see langword="false"/> otherwise. </returns>
        bool HasFeature(string feature, string version);

        /// <summary>
        /// Creates an empty <see cref="IDocumentType"/> node. Entity declarations and notations are not made available. 
        /// </summary>
        /// <remarks>
        /// Entity reference expansions and default attribute additions do not occur. It is expected that a 
        /// future version of the DOM will provide a way for populating a <see cref="IDocumentType"/>.
        /// HTML-only DOM implementations do not need to implement this method. 
        /// </remarks>
        /// <param name="qualifiedName"> qualified name of the document type to be created. </param>
        /// <param name="publicId"> external subset public identifier. </param>
        /// <param name="systemId"> external subset system identifier. </param>
        /// <returns> A new <see cref="IDocumentType"/> node with <see cref="IXmlNode.OwnerDocument"/> 
        /// set to <see langword="null"/>. </returns>
        /// <exception cref="DomException">
        /// INVALID_CHARACTER_ERR: Raised if the specified qualified name contains an illegal character.
        /// <para>
        /// NAMESPACE_ERR: Raised if the <paramref name="qualifiedName"/> is malformed.
        /// </para>
        /// </exception>
        IDocumentType CreateDocumentType(string qualifiedName, string publicId, string systemId);

        /// <summary>
        /// Creates an XML <see cref="IDocument"/> object of the specified type with 
        /// its document element. HTML-only DOM implementations do not need to 
        /// implement this method. 
        /// </summary>
        /// <param name="namespaceURI"> namespace URI of the document element to create. </param>
        /// <param name="qualifiedName"> qualified name of the document element to be created. </param>
        /// <param name="doctype"> type of document to be created or <see langword="null"/>. 
        /// When <paramref name="doctype"/> is not <see langword="null"/>, its <see cref="IXmlNode.OwnerDocument"/> 
        /// attribute is set to the document being created.
        /// </param>
        /// <returns> A new <see cref="IDocument"/> object. </returns>
        /// <exception cref="DomException">
        /// <para>INVALID_CHARACTER_ERR: Raised if the specified qualified name contains an illegal character.
        /// </para>
        /// <para>NAMESPACE_ERR: Raised if the <paramref name="qualifiedName"/> is malformed, 
        /// if the <paramref name="qualifiedName"/> has a prefix and the <paramref name="namespaceURI"/> 
        /// is <see langword="null"/>, or if the <paramref name="qualifiedName"/> has a prefix that is "xml" and the
        /// <paramref name="namespaceURI"/> is different from <see href="https://www.w3.org/XML/1998/namespace"/>.
        /// </para>
        /// WRONG_DOCUMENT_ERR: Raised if <paramref name="doctype"/> has already been used with a different 
        /// document or was created from a different implementation.
        /// </exception>
        IDocument CreateDocument(string namespaceURI, string qualifiedName, IDocumentType doctype);
    }
}
