using System.Xml;

namespace SharpVectors.Dom
{
    /// <summary>
    /// The <see cref="IElement"/> interface represents an element in an HTML or XML document. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// Elements may have attributes associated with them; since the <see cref="IElement"/> interface inherits 
    /// from <see cref="INode"/>, the generic <see cref="INode"/> interface attribute <see cref="IAttribute"/> may 
    /// be used to retrieve the set of all attributes for an element. 
    /// </para>
    /// <para>
    /// There are methods on the <see cref="IElement"/> interface to retrieve either an <see cref="IAttribute"/> object 
    /// by name or an attribute value by name. In XML, where an attribute value may contain entity references, an 
    /// <see cref="IAttribute"/> object should be retrieved to examine the possibly fairly complex sub-tree representing 
    /// the attribute value. 
    /// </para>
    /// <para>
    /// On the other hand, in HTML, where all attributes have simple string values, methods to directly access an attribute 
    /// value can safely be used as a convenience. In DOM Level 2, the method <see cref="IXmlNode.Normalize"/> is inherited from the 
    /// <see cref="INode"/> interface where it was moved.
    /// </para>
    /// </remarks>
    /// <seealso href="http://www.w3.org/TR/2000/REC-DOM-Level-2-Core-20001113">
    /// Document Object Model (DOM) Level 2 Core Specification</seealso>
    public interface IXmlElement : INode
	{
		/// <summary>
		/// Gets a value indicating whether this node (if it is an element) has any attributes. 
		/// </summary>
		/// <value><see langword="true"/> if this node has any attributes, <see langword="false"/> otherwise.</value>
		bool HasAttributes
		{
			get;
		}

        /// <summary>
        /// Returns the value for the attribute with the specified name.
        /// </summary>
        /// <param name="name">The name of the attribute to retrieve. This is a qualified name. 
        /// It is matched against the <see cref="IXmlNode.Name"/> property of the matching node.
        /// </param>
        /// <returns>The value of the specified attribute. An empty string is returned if a matching
        /// attribute is not found or if the attribute does not have a specified or default value.
        /// </returns>
        string GetAttribute(string name);

		/// <summary>
		/// Returns the value for the attribute with the specified local name and namespace URI.
		/// </summary>
		/// <param name="localName">The local name of the attribute to retrieve.</param>
		/// <param name="namespaceURI">The namespace URI of the attribute to retrieve.</param>
		/// <returns>The value of the specified attribute. An empty string is returned if a matching
		/// attribute is not found or if the attribute does not have a specified or default value.
		/// </returns>
		string GetAttribute(string localName, string namespaceURI);

		/// <summary>
		/// Sets the value of the attribute with the specified name.
		/// </summary>
		/// <param name="name">The name of the attribute to create or alter. This is a qualified name. If the
		/// name contains a colon it is parsed into prefix and local name components.
		/// </param>
		/// <param name="value">The value to set for the attribute.</param>
		void SetAttribute(string name, string value);

		/// <summary>
		/// Sets the value of the attribute with the specified local name and namespace URI.
		/// </summary>
		/// <param name="qualifiedName">The local name of the attribute.</param>
		/// <param name="namespaceURI">The namespace URI of the attribute.</param>
		/// <param name="value">The value to set for the attribute.</param>
		/// <returns>The attribute value.</returns>
		string SetAttribute(string qualifiedName, string namespaceURI, string value);

		/// <summary>
		/// Removes an attribute by name.
		/// </summary>
		/// <param name="name">The name of the attribute to remove.This is a qualified name. 
		/// It is matched against the Name property of the matching node.
		/// </param>
		void RemoveAttribute(string name);

		/// <summary>
		/// Removes an attribute with the specified local name and namespace URI. (If the
		/// removed attribute has a default value, it is immediately replaced).
		/// </summary>
		/// <param name="localName">The local name of the attribute to remove.</param>
		/// <param name="namespaceURI">The namespace URI of the attribute to remove.</param>
		void RemoveAttribute(string localName, string namespaceURI);

		/// <summary>
		/// Returns the <see cref="XmlAttribute"/> with the specified name.
		/// </summary>
		/// <param name="name">The name of the attribute to retrieve. This is a qualified name. 
		/// It is matched against the Name property of the matching node.
		/// </param>
		/// <returns>The specified <see cref="XmlAttribute"/> or <see langword="null"/> if a matching attribute was not found.</returns>
		/// <see cref="IAttribute"/>
		XmlAttribute GetAttributeNode(string name);

		/// <summary>
		/// Returns the <see cref="XmlAttribute"/> with the specified local name and namespace URI.
		/// </summary>
		/// <param name="localName">The local name of the attribute.</param>
		/// <param name="namespaceURI">The namespace URI of the attribute.</param>
		/// <returns>The specified <see cref="XmlAttribute"/> or <see langword="null"/> if a matching attribute was not found.</returns>
		/// <see cref="IAttribute"/>
		XmlAttribute GetAttributeNode(string localName,	string namespaceURI);

		/// <summary>
		/// Returns an <see cref="XmlNodeList"/> containing a list of all descendant elements
		/// that match the specified <see cref="XmlElement.Name"/>.
		/// </summary>
		/// <param name="name">The name tag to match. This is a qualified name. It is matched against the Name
		/// property of the matching node. The asterisk (*) is a special value that matches all tags.
		/// </param>
		/// <returns>An <see cref="XmlNodeList"/> containing a list of all matching nodes.</returns>
		/// <seealso cref="INodeList"/>
		XmlNodeList GetElementsByTagName(string name);

		/// <summary>
		/// Returns an <see cref="XmlNodeList"/> containing a list of all descendant elements
		/// that match the specified <see cref="XmlElement.LocalName"/> and <see cref="XmlElement.NamespaceURI"/>.
		/// </summary>
		/// <param name="localName">The local name to match. The asterisk (*) is a special value that matches all tags.</param>
		/// <param name="namespaceURI">The namespace URI to match.</param>
		/// <returns>An <see cref="XmlNodeList"/> containing a list of all matching nodes.</returns>
		/// <seealso cref="INodeList"/>
		XmlNodeList GetElementsByTagName(string localName, string namespaceURI);

		/// <summary>
		/// Adds the specified <see cref="XmlAttribute"/>.
		/// </summary>
		/// <param name="localName">The local name of the attribute.</param>
		/// <param name="namespaceURI">The namespace URI of the attribute.</param>
		/// <returns>The <see cref="XmlAttribute"/> to add.</returns>
		XmlAttribute SetAttributeNode(string localName, string namespaceURI);

		/// <summary>
		/// Adds the specified <see cref="XmlAttribute"/>.
		/// </summary>
		/// <param name="newAttr">The <see cref="XmlAttribute"/> node to add to the attribute collection for this element.</param>
		/// <returns>
		/// If the attribute replaces an existing attribute with the same name, the old <see cref="XmlAttribute"/>
		/// is returned; otherwise, <see langword="null"/> is returned.
		/// </returns>
		/// <see cref="IAttribute"/>
		XmlAttribute SetAttributeNode(XmlAttribute newAttr);

		/// <summary>
		/// Removes the specified <see cref="XmlAttribute"/>.
		/// </summary>
		/// <param name="oldAttr">
		/// The <see cref="XmlAttribute"/> node to remove. If the removed attribute has a default value,
		/// it is immediately replaced.
		/// </param>
		/// <returns>The removed <see cref="XmlAttribute"/> or <see langword="null"/> if <paramref name="oldAttr"/> 
		/// is not an attribute node of the <see cref="XmlElement"/> .</returns>
		/// <see cref="IAttribute"/>
		XmlAttribute RemoveAttributeNode(XmlAttribute oldAttr);

		/// <summary>
		/// Removes the <see cref="XmlAttribute"/> specified by the local name and namespace
		/// URI. (If the removed attribute has a default value, it is immediately replaced).
		/// </summary>
		/// <param name="localName">The local name of the attribute.</param>
		/// <param name="namespaceURI">The namespace URI of the attribute.</param>
		/// <returns>
		/// The removed <see cref="XmlAttribute"/> or <see langword="null"/> if the <see cref="XmlElement"/> 
		/// does not have a matching attribute node.
		/// </returns>
		/// <see cref="IAttribute"/>
		XmlAttribute RemoveAttributeNode(string localName, string namespaceURI);

		/// <summary>
		/// Determines whether the current node has an attribute with the specified name.
		/// </summary>
		/// <param name="name">
		/// The name of the attribute to find. This is a qualified name. It is matched against
		/// the Name property of the matching node.
		/// </param>
		/// <returns><see langword="true"/> if the current node has the specified attribute; otherwise, <see langword="false"/>.</returns>
		/// <see cref="IAttribute"/>
		bool HasAttribute(string name);

		/// <summary>
		/// Determines whether the current node has an attribute with the specified local
		/// name and namespace URI.
		/// </summary>
		/// <param name="localName">The local name of the attribute to find.</param>
		/// <param name="namespaceURI">The namespace URI of the attribute to find.</param>
		/// <returns><see langword="true"/> if the current node has the specified attribute; otherwise, <see langword="false"/>.</returns>
		/// <see cref="IAttribute"/>
		bool HasAttribute(string localName, string namespaceURI);
	}
}
