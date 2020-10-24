using System;
using System.Xml;
using System.Collections;

namespace SharpVectors.Dom
{
    /// <summary>
    /// Represents a collection of nodes that can be accessed by name or index.
    /// <para>
    /// Objects implementing the <see cref="INamedNodeMap"/> interface are used to represent collections of nodes 
    /// that can be accessed by name. 
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Note that <see cref="INamedNodeMap"/> does not inherit from <see cref="INodeList"/>; <see cref="INamedNodeMap"/>s 
    /// are not maintained in any particular order. Objects contained in an object implementing <see cref="INamedNodeMap"/> 
    /// may also be accessed by an ordinal index, but this is simply to allow convenient enumeration of the contents 
    /// of a <see cref="INamedNodeMap"/>, and does not imply that the DOM specifies an order to these Nodes. 
    /// </para>
    /// <para><see cref="INamedNodeMap"/> objects in the DOM are live.</para>
    /// </remarks>
    /// <seealso href="http://www.w3.org/TR/2000/REC-DOM-Level-2-Core-20001113">
    /// Document Object Model (DOM) Level 2 Core Specification</seealso>
    /// <seealso cref="XmlAttributeCollection"/>
    public interface INamedNodeMap : IEnumerable
    {
        /// <summary>
        /// Gets the number of nodes in the <see cref="INamedNodeMap"/>.
        /// </summary>
        /// <value>The number of nodes.</value>
        int Count { get; }

        /// <summary>
        /// Retrieves an <see cref="XmlNode"/> specified by name.
        /// </summary>
        /// <param name="name">
        /// The qualified name of the node to retrieve. It is matched against the <see cref="XmlNode.Name"/>
        /// property of the matching node.
        /// </param>
        /// <returns>An <see cref="XmlNode"/> with the specified name or <see langword="null"/> if a matching node is not found.</returns>
        XmlNode GetNamedItem(string name);

        /// <summary>
        /// Retrieves a node with the matching <see cref="XmlNode.LocalName"/> and <see cref="XmlNode.NamespaceURI"/>.
        /// </summary>
        /// <param name="localName">The local name of the node to retrieve.</param>
        /// <param name="namespaceURI">The namespace Uniform Resource Identifier (URI) of the node to retrieve.</param>
        /// <returns>
        /// An <see cref="XmlNode"/> with the matching local name and namespace URI or <see langword="null"/>
        /// if a matching node was not found.
        /// </returns>
        XmlNode GetNamedItem(string localName, string namespaceURI);

        /// <summary>
        /// Retrieves the node at the specified index in the <see cref="INamedNodeMap"/>.
        /// </summary>
        /// <param name="index">
        /// The index position of the node to retrieve from the <see cref="INamedNodeMap"/>. The index
        /// is zero-based; therefore, the index of the first node is 0 and the index of the
        /// last node is <see cref="INamedNodeMap.Count"/> -1.
        /// </param>
        /// <returns>
        /// The <see cref="XmlNode"/> at the specified index. If index is less than 0 or greater
        /// than or equal to the <see cref="INamedNodeMap.Count"/> property, <see langword="null"/> is returned.
        /// </returns>
        XmlNode Item(int index);

        /// <summary>
        /// Removes the node from the <see cref="INamedNodeMap"/>.
        /// </summary>
        /// <param name="name">
        /// The qualified name of the node to remove. The name is matched against the <see cref="XmlNode.Name"/>
        /// property of the matching node.
        /// </param>
        /// <returns>The <see cref="XmlNode"/> removed from this <see cref="INamedNodeMap"/> 
        /// or <see langword="null"/> if a matching node was not found.</returns>
        XmlNode RemoveNamedItem(string name);

        /// <summary>
        /// Removes a node with the matching <see cref="XmlNode.LocalName"/> and <see cref="XmlNode.NamespaceURI"/>.
        /// </summary>
        /// <param name="localName">The local name of the node to remove.</param>
        /// <param name="namespaceURI">The namespace URI of the node to remove.</param>
        /// <returns>The <see cref="XmlNode"/> removed or <see langword="null"/> if a matching node was not found.</returns>
        XmlNode RemoveNamedItem(string localName, string namespaceURI);

        /// <summary>
        /// Adds an <see cref="XmlNode"/> using its <see cref="XmlNode.Name"/> property
        /// </summary>
        /// <param name="node">An <see cref="XmlNode"/> to store in the <see cref="INamedNodeMap"/>. If a node with that name is already
        /// present in the map, it is replaced by the new one.</param>
        /// <returns>
        /// If the node replaces an existing node with the same name, the old node is returned;
        /// otherwise, <see langword="null"/> is returned.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The node was created from a different <see cref="XmlDocument"/> than the one that
        /// created the <see cref="INamedNodeMap"/>; or the <see cref="INamedNodeMap"/> is read-only.
        /// </exception>
        XmlNode SetNamedItem(XmlNode node);
    }
}
