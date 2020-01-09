using System;
using System.Xml;
using System.Collections;

namespace SharpVectors.Dom
{
    public interface INamedNodeMap : IEnumerable
    {
        //
        // Summary:
        //     Gets the number of nodes in the XmlNamedNodeMap.
        //
        // Returns:
        //     The number of nodes.
        int Count { get; }

        //
        // Summary:
        //     Retrieves an System.Xml.XmlNode specified by name.
        //
        // Parameters:
        //   name:
        //     The qualified name of the node to retrieve. It is matched against the System.Xml.XmlNode.Name
        //     property of the matching node.
        //
        // Returns:
        //     An XmlNode with the specified name or null if a matching node is not found.
        XmlNode GetNamedItem(string name);
        //
        // Summary:
        //     Retrieves a node with the matching System.Xml.XmlNode.LocalName and System.Xml.XmlNode.NamespaceURI.
        //
        // Parameters:
        //   localName:
        //     The local name of the node to retrieve.
        //
        //   namespaceURI:
        //     The namespace Uniform Resource Identifier (URI) of the node to retrieve.
        //
        // Returns:
        //     An System.Xml.XmlNode with the matching local name and namespace URI or null
        //     if a matching node was not found.
        XmlNode GetNamedItem(string localName, string namespaceURI);
        //
        // Summary:
        //     Retrieves the node at the specified index in the XmlNamedNodeMap.
        //
        // Parameters:
        //   index:
        //     The index position of the node to retrieve from the XmlNamedNodeMap. The index
        //     is zero-based; therefore, the index of the first node is 0 and the index of the
        //     last node is System.Xml.XmlNamedNodeMap.Count -1.
        //
        // Returns:
        //     The System.Xml.XmlNode at the specified index. If index is less than 0 or greater
        //     than or equal to the System.Xml.XmlNamedNodeMap.Count property, null is returned.
        XmlNode Item(int index);
        //
        // Summary:
        //     Removes the node from the XmlNamedNodeMap.
        //
        // Parameters:
        //   name:
        //     The qualified name of the node to remove. The name is matched against the System.Xml.XmlNode.Name
        //     property of the matching node.
        //
        // Returns:
        //     The XmlNode removed from this XmlNamedNodeMap or null if a matching node was
        //     not found.
        XmlNode RemoveNamedItem(string name);
        //
        // Summary:
        //     Removes a node with the matching System.Xml.XmlNode.LocalName and System.Xml.XmlNode.NamespaceURI.
        //
        // Parameters:
        //   localName:
        //     The local name of the node to remove.
        //
        //   namespaceURI:
        //     The namespace URI of the node to remove.
        //
        // Returns:
        //     The System.Xml.XmlNode removed or null if a matching node was not found.
        XmlNode RemoveNamedItem(string localName, string namespaceURI);
        //
        // Summary:
        //     Adds an System.Xml.XmlNode using its System.Xml.XmlNode.Name property
        //
        // Parameters:
        //   node:
        //     An XmlNode to store in the XmlNamedNodeMap. If a node with that name is already
        //     present in the map, it is replaced by the new one.
        //
        // Returns:
        //     If the node replaces an existing node with the same name, the old node is returned;
        //     otherwise, null is returned.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     The node was created from a different System.Xml.XmlDocument than the one that
        //     created the XmlNamedNodeMap; or the XmlNamedNodeMap is read-only.
        XmlNode SetNamedItem(XmlNode node);
    }
}
