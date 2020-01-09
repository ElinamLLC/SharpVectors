using System;

using SharpVectors.Dom;

namespace SharpVectors.Scripting
{
    /// <summary>
    /// IJsDomTimeStamp
    /// </summary>
    public interface IJsDomTimeStamp : IScriptableObject<IDomTimeStamp>
    {
    }

    /// <summary>
    /// IJsDomImplementation
    /// </summary>
    public interface IJsDomImplementation : IScriptableObject<IDomImplementation>
    {
        bool hasFeature(string feature, string version);
        IJsDocumentType createDocumentType(string qualifiedName, string publicId, string systemId);
        IJsDocument createDocument(string namespaceURI, string qualifiedName, IJsDocumentType doctype);
    }

    /// <summary>
    /// IJsNode
    /// </summary>
    public interface IJsNode : IScriptableObject<INode>
    {
        string nodeName { get; }
        string nodeValue { get; set; }
        ushort nodeType { get; }
        IJsNode parentNode { get; }
        IJsNodeList childNodes { get; }
        IJsNode firstChild { get; }
        IJsNode lastChild { get; }
        IJsNode previousSibling { get; }
        IJsNode nextSibling { get; }
        IJsNamedNodeMap attributes { get; }
        IJsDocument ownerDocument { get; }
        string namespaceURI { get; }
        string prefix { get; set; }
        string localName { get; }

        IJsNode insertBefore(IJsNode newChild, IJsNode refChild);
        IJsNode replaceChild(IJsNode newChild, IJsNode oldChild);
        IJsNode removeChild(IJsNode oldChild);
        IJsNode appendChild(IJsNode newChild);
        bool hasChildNodes();
        IJsNode cloneNode(bool deep);
        void normalize();
        bool isSupported(string feature, string version);
        bool hasAttributes();
    }

    /// <summary>
    /// IJsNodeList
    /// </summary>
    public interface IJsNodeList : IScriptableObject
    {
        IJsNode item(ulong index);
        ulong length { get; }
    }

    /// <summary>
    /// IJsNamedNodeMap
    /// </summary>
    public interface IJsNamedNodeMap : IScriptableObject
    {
        ulong length { get; }

        IJsNode getNamedItem(string name);
        IJsNode setNamedItem(IJsNode arg);
        IJsNode removeNamedItem(string name);
        IJsNode item(ulong index);
        IJsNode getNamedItemNS(string namespaceURI, string localName);
        IJsNode setNamedItemNS(IJsNode arg);
        IJsNode removeNamedItemNS(string namespaceURI, string localName);
    }

    /// <summary>
    /// IJsCharacterData
    /// </summary>
    public interface IJsCharacterData : IJsNode
    {
        string data { get; set; }
        ulong length { get; }

        string substringData(ulong offset, ulong count);
        void appendData(string arg);
        void insertData(ulong offset, string arg);
        void deleteData(ulong offset, ulong count);
        void replaceData(ulong offset, ulong count, string arg);
    }

    /// <summary>
    /// IJsAttr
    /// </summary>
    public interface IJsAttr : IJsNode
    {
        string name { get; }
        bool specified { get; }
        string value { get; set; }
        IJsElement ownerElement { get; }
    }

    /// <summary>
    /// IJsElement
    /// </summary>
    public interface IJsElement : IJsNode
    {
        string tagName { get; }

        string getAttribute(string name);
        void setAttribute(string name, string value);
        void removeAttribute(string name);
        IJsAttr getAttributeNode(string name);
        IJsAttr setAttributeNode(IJsAttr newAttr);
        IJsAttr removeAttributeNode(IJsAttr oldAttr);
        IJsNodeList getElementsByTagName(string name);
        string getAttributeNS(string namespaceURI, string localName);
        void setAttributeNS(string namespaceURI, string qualifiedName, string value);
        void removeAttributeNS(string namespaceURI, string localName);
        IJsAttr getAttributeNodeNS(string namespaceURI, string localName);
        IJsAttr setAttributeNodeNS(IJsAttr newAttr);
        IJsNodeList getElementsByTagNameNS(string namespaceURI, string localName);
        bool hasAttribute(string name);
        bool hasAttributeNS(string namespaceURI, string localName);
    }

    /// <summary>
    /// IJsText
    /// </summary>
    public interface IJsText : IJsCharacterData
    {
        IJsText splitText(ulong offset);
    }

    /// <summary>
    /// IJsComment
    /// </summary>
    public interface IJsComment : IJsCharacterData
    {
    }

    /// <summary>
    /// IJsCDataSection
    /// </summary>
    public interface IJsCDataSection : IJsCharacterData
    {
    }

    /// <summary>
    /// IJsDocumentType
    /// </summary>
    public interface IJsDocumentType : IJsNode
    {
        string name { get; }
        IJsNamedNodeMap entities { get; }
        IJsNamedNodeMap notations { get; }
        string publicId { get; }
        string systemId { get; }
        string internalSubset { get; }
    }

    /// <summary>
    /// IJsNotation
    /// </summary>
    public interface IJsNotation : IJsNode
    {
        string publicId { get; }
        string systemId { get; }
    }

    /// <summary>
    /// IJsEntity
    /// </summary>
    public interface IJsEntity : IJsNode
    {
        string publicId { get; }
        string systemId { get; }
        string notationName { get; }
    }

    /// <summary>
    /// IJsEntityReference
    /// </summary>
    public interface IJsEntityReference : IJsNode
    {
    }

    /// <summary>
    /// IJsProcessingInstruction
    /// </summary>
    public interface IJsProcessingInstruction : IJsNode
    {
        string target { get; }
        string data { get; set; }
    }

    /// <summary>
    /// IJsDocumentFragment
    /// </summary>
    public interface IJsDocumentFragment : IJsNode
    {
    }

    /// <summary>
    /// IJsDocument
    /// </summary>
    public interface IJsDocument : IJsNode
    {
        IJsDocumentType doctype { get; }
        IJsDomImplementation implementation { get; }
        IJsElement documentElement { get; }

        IJsElement createElement(string tagName);
        IJsDocumentFragment createDocumentFragment();
        IJsText createTextNode(string data);
        IJsComment createComment(string data);
        IJsCDataSection createCDATASection(string data);
        IJsProcessingInstruction createProcessingInstruction(string target, string data);
        IJsAttr createAttribute(string name);
        IJsEntityReference createEntityReference(string name);
        IJsNodeList getElementsByTagName(string tagname);
        IJsNode importNode(IJsNode importedNode, bool deep);
        IJsElement createElementNS(string namespaceURI, string qualifiedName);
        IJsAttr createAttributeNS(string namespaceURI, string qualifiedName);
        IJsNodeList getElementsByTagNameNS(string namespaceURI, string localName);
        IJsElement getElementById(string elementId);
    }   
}
