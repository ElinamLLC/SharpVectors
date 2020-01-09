using System;
using System.Xml;

using SharpVectors.Dom;

namespace SharpVectors.Scripting
{
    #region Implementation - IJsDomTimeStamp: TODO

    /// <summary>
    /// IJsDomTimeStamp
    /// </summary>
    public sealed class JsDomTimeStamp : JsObject<IDomTimeStamp>, IJsDomTimeStamp
    {
        public JsDomTimeStamp(IDomTimeStamp baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }
    }

    #endregion

    #region Implementation - IJsDomImplementation: TODO

    /// <summary>
    /// Implementation wrapper for IJsDomImplementation
    /// </summary>
    public class JsDomImplementation : JsObject<IDomImplementation>, IJsDomImplementation
    {
        public JsDomImplementation(IDomImplementation baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public bool hasFeature(string feature, string version)
        {
            return ((XmlImplementation)_baseObject).HasFeature(feature, version);
        }

        public IJsDocumentType createDocumentType(string qualifiedName, string publicId, string systemId)
        {
            throw new NotSupportedException();
            //var result = ((XmlImplementation)baseObject).CreateDocumentType(qualifiedName, publicId, systemId);
            //return (result != null) ? (IJsDocumentType)JsObject.CreateWrapper<>(result, _engine) : null;
        }

        public IJsDocument createDocument(string namespaceURI, string qualifiedName, IJsDocumentType doctype)
        {
            throw new NotSupportedException();
            //var result = ((XmlImplementation)baseObject).CreateDocument(namespaceURI, qualifiedName, ((IDocumentType)((JsDocumentType)doctype).baseObject));
            //return (result != null) ? (IJsDocument)JsObject.CreateWrapper<>(result, _engine) : null;
        }
    }

    #endregion

    #region Implementation - IJsNode

    /// <summary>
    /// Implementation wrapper for IJsNode
    /// </summary>
    public class JsNode : JsObject<INode>, IJsNode
    {
        public JsNode(INode baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsNode insertBefore(IJsNode newChild, IJsNode refChild)
        {
            var result = _baseObject.InsertBefore((XmlNode)newChild.BaseObject, (XmlNode)refChild.BaseObject);
            return (result != null) ? CreateWrapper<IJsNode>(result, _engine) : null;
        }

        public IJsNode replaceChild(IJsNode newChild, IJsNode oldChild)
        {
            var result = _baseObject.ReplaceChild((XmlNode)newChild.BaseObject, (XmlNode)oldChild.BaseObject);
            return (result != null) ? CreateWrapper<IJsNode>(result, _engine) : null;
        }

        public IJsNode removeChild(IJsNode oldChild)
        {
            var result = _baseObject.RemoveChild(((XmlNode)((JsNode)oldChild).BaseObject));
            return (result != null) ? CreateWrapper<IJsNode>(result, _engine) : null;
        }

        public IJsNode appendChild(IJsNode newChild)
        {
            var result = _baseObject.AppendChild(((XmlNode)((JsNode)newChild).BaseObject));
            return (result != null) ? CreateWrapper<IJsNode>(result, _engine) : null;
        }

        public bool hasChildNodes()
        {
            return _baseObject.HasChildNodes;
        }

        public IJsNode cloneNode(bool deep)
        {
            var result = _baseObject.CloneNode(deep);
            return (result != null) ? CreateWrapper<IJsNode>(result, _engine) : null;
        }

        public void normalize()
        {
            _baseObject.Normalize();
        }

        public bool isSupported(string feature, string version)
        {
            return _baseObject.Supports(feature, version);
        }

        public bool hasAttributes()
        {
            return _baseObject.Attributes.Count > 0;
        }

        public string nodeName
        {
            get { return _baseObject.Name; }
        }

        public string nodeValue
        {
            get { return _baseObject.Value; }
            set { _baseObject.Value = value; }
        }

        public ushort nodeType
        {
            get { return (ushort)_baseObject.NodeType; }
        }

        public IJsNode parentNode
        {
            get {
                var result = _baseObject.ParentNode;
                return (result != null) ? CreateWrapper<IJsNode>(result, _engine) : null;
            }
        }

        public IJsNodeList childNodes
        {
            get {
                var result = _baseObject.ChildNodes;
                return (result != null) ? CreateWrapper<IJsNodeList>(result, _engine) : null;
            }
        }

        public IJsNode firstChild
        {
            get {
                var result = _baseObject.FirstChild;
                return (result != null) ? CreateWrapper<IJsNode>(result, _engine) : null;
            }
        }

        public IJsNode lastChild
        {
            get {
                var result = _baseObject.LastChild;
                return (result != null) ? CreateWrapper<IJsNode>(result, _engine) : null;
            }
        }

        public IJsNode previousSibling
        {
            get {
                var result = _baseObject.PreviousSibling;
                return (result != null) ? CreateWrapper<IJsNode>(result, _engine) : null;
            }
        }

        public IJsNode nextSibling
        {
            get {
                var result = _baseObject.NextSibling;
                return (result != null) ? CreateWrapper<IJsNode>(result, _engine) : null;
            }
        }

        public IJsNamedNodeMap attributes
        {
            get {
                var result = _baseObject.Attributes;
                return (result != null) ? CreateWrapper<IJsNamedNodeMap>(result, _engine) : null;
            }
        }

        public IJsDocument ownerDocument
        {
            get {
                var result = _baseObject.OwnerDocument;
                return (result != null) ? CreateWrapper<IJsDocument>(result, _engine) : null;
            }
        }

        public string namespaceURI
        {
            get { return _baseObject.NamespaceURI; }
        }

        public string prefix
        {
            get { return _baseObject.Prefix; }
            set { _baseObject.Prefix = value; }
        }

        public string localName
        {
            get { return _baseObject.LocalName; }
        }
    }

    #endregion

    #region Implementation - IJsNodeList

    /// <summary>
    /// Implementation wrapper for IJsNodeList
    /// </summary>
    public sealed class JsNodeList : JsObject<INodeList>, IJsNodeList
    {
        public JsNodeList(INodeList baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsNode item(ulong index)
        {
            var result = _baseObject[index];
            return (result != null) ? CreateWrapper<IJsNode>(result, _engine) : null;
        }

        public ulong length
        {
            get { return _baseObject.Count; }
        }
    }

    #endregion

    #region Implementation - IJsNamedNodeMap: TODO

    /// <summary>
    /// Implementation wrapper for IJsNamedNodeMap
    /// </summary>
    public sealed class JsNamedNodeMap : JsObject<INamedNodeMap>, IJsNamedNodeMap
    {
        public JsNamedNodeMap(INamedNodeMap baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsNode getNamedItem(string name)
        {
            var result = ((XmlNamedNodeMap)_baseObject).GetNamedItem(name);
            return (result != null) ? CreateWrapper<IJsNode>(result, _engine) : null;
        }

        public IJsNode setNamedItem(IJsNode arg)
        {
            var result = ((XmlNamedNodeMap)_baseObject).SetNamedItem((XmlNode)arg.BaseObject);
            return (result != null) ? CreateWrapper<IJsNode>(result, _engine) : null;
        }

        public IJsNode removeNamedItem(string name)
        {
            var result = ((XmlNamedNodeMap)_baseObject).RemoveNamedItem(name);
            return (result != null) ? CreateWrapper<IJsNode>(result, _engine) : null;
        }

        public IJsNode item(ulong index)
        {
            var result = ((XmlNamedNodeMap)_baseObject).Item((int)index);
            return (result != null) ? CreateWrapper<IJsNode>(result, _engine) : null;
        }

        public IJsNode getNamedItemNS(string namespaceURI, string localName)
        {
            var result = ((XmlNamedNodeMap)_baseObject).GetNamedItem(localName, namespaceURI == null ? "" : namespaceURI);
            return (result != null) ? CreateWrapper<IJsNode>(result, _engine) : null;
        }

        public IJsNode setNamedItemNS(IJsNode arg)
        {
            var result = ((XmlNamedNodeMap)_baseObject).SetNamedItem((XmlNode)arg.BaseObject);
            return (result != null) ? CreateWrapper<IJsNode>(result, _engine) : null;
        }

        public IJsNode removeNamedItemNS(string namespaceURI, string localName)
        {
            var result = ((XmlNamedNodeMap)_baseObject).RemoveNamedItem(localName, namespaceURI == null ? "" : namespaceURI);
            return (result != null) ? CreateWrapper<IJsNode>(result, _engine) : null;
        }

        public ulong length
        {
            get { return (ulong)((XmlNamedNodeMap)_baseObject).Count; }
        }
    }

    #endregion

    #region Implementation - IJsCharacterData

    /// <summary>
    /// Implementation wrapper for IJsCharacterData
    /// </summary>
    public class JsCharacterData : JsNode, IJsCharacterData
    {
        public JsCharacterData(ICharacterData baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public string substringData(ulong offset, ulong count)
        {
            if (_baseObject.Value != null)
                return _baseObject.Value.Substring((int)offset, (int)count);

            return null;
        }

        public void appendData(string arg)
        {
            _baseObject.Value += arg;
        }

        public void insertData(ulong offset, string arg)
        {
            if (_baseObject.Value != null)
                _baseObject.Value.Insert((int)offset, arg);
            else
                _baseObject.Value = arg;
        }

        public void deleteData(ulong offset, ulong count)
        {
            if (_baseObject.Value != null)
                _baseObject.Value.Remove((int)offset, (int)count);
        }

        public void replaceData(ulong offset, ulong count, string arg)
        {
            deleteData(offset, count);
            insertData(offset, arg);
        }

        public string data
        {
            get { return _baseObject.Value; }
            set { _baseObject.Value = value; }
        }

        public ulong length
        {
            get { return (_baseObject.Value != null) ? (ulong)_baseObject.Value.Length : 0; }
        }
    }

    #endregion

    #region Implementation - IJsAttr

    /// <summary>
    /// Implementation wrapper for IJsAttr
    /// </summary>
    public sealed class JsAttr : JsNode, IJsAttr
    {
        public JsAttr(IAttribute baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public string name
        {
            get { return _baseObject.Name; }
        }

        public bool specified
        {
            get { return ((IAttribute)_baseObject).Specified; }
        }

        public string value
        {
            get { return _baseObject.Value; }
            set { _baseObject.Value = value; }
        }

        public IJsElement ownerElement
        {
            get {
                var result = _baseObject.ParentNode;
                return (result != null) ? CreateWrapper<IJsElement>(result, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsElement

    /// <summary>
    /// Implementation wrapper for IJsElement
    /// </summary>
    public class JsElement : JsNode, IJsElement
    {
        public JsElement(IElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public string getAttribute(string name)
        {
            return ((IElement)_baseObject).GetAttribute(name);
        }

        public void setAttribute(string name, string value)
        {
            ((IElement)_baseObject).SetAttribute(name, value);
        }

        public void removeAttribute(string name)
        {
            ((IElement)_baseObject).RemoveAttribute(name);
        }

        public IJsAttr getAttributeNode(string name)
        {
            var result = ((IElement)_baseObject).GetAttributeNode(name);
            return (result != null) ? CreateWrapper<IJsAttr>(result, _engine) : null;
        }

        public IJsAttr setAttributeNode(IJsAttr newAttr)
        {
            var result = ((IElement)_baseObject).SetAttributeNode((XmlAttribute)newAttr.BaseObject);
            return (result != null) ? CreateWrapper<IJsAttr>(result, _engine) : null;
        }

        public IJsAttr removeAttributeNode(IJsAttr oldAttr)
        {
            var result = ((IElement)_baseObject).RemoveAttributeNode((XmlAttribute)oldAttr.BaseObject);
            return (result != null) ? CreateWrapper<IJsAttr>(result, _engine) : null;
        }

        public IJsNodeList getElementsByTagName(string name)
        {
            var result = ((IElement)_baseObject).GetElementsByTagName(name);
            return (result != null) ? CreateWrapper<IJsNodeList>(result, _engine) : null;
        }

        public string getAttributeNS(string namespaceURI, string localName)
        {
            return ((IElement)_baseObject).GetAttribute(localName, namespaceURI == null ? "" : namespaceURI);
        }

        public void setAttributeNS(string namespaceURI, string qualifiedName, string value)
        {
            ((IElement)_baseObject).SetAttribute(qualifiedName, namespaceURI == null ? "" : namespaceURI, value);
        }

        public void removeAttributeNS(string namespaceURI, string localName)
        {
            ((IElement)_baseObject).RemoveAttribute(localName, namespaceURI == null ? "" : namespaceURI);
        }

        public IJsAttr getAttributeNodeNS(string namespaceURI, string localName)
        {
            var result = ((IElement)_baseObject).GetAttributeNode(localName, namespaceURI == null ? "" : namespaceURI);
            return (result != null) ? CreateWrapper<IJsAttr>(result, _engine) : null;
        }

        public IJsAttr setAttributeNodeNS(IJsAttr newAttr)
        {
            var result = ((IElement)_baseObject).SetAttributeNode((XmlAttribute)newAttr.BaseObject);
            return (result != null) ? CreateWrapper<IJsAttr>(result, _engine) : null;
        }

        public IJsNodeList getElementsByTagNameNS(string namespaceURI, string localName)
        {
            var result = ((IElement)_baseObject).GetElementsByTagName(localName, namespaceURI == null ? "" : namespaceURI);
            return (result != null) ? CreateWrapper<IJsNodeList>(result, _engine) : null;
        }

        public bool hasAttribute(string name)
        {
            return ((IElement)_baseObject).HasAttribute(name);
        }

        public bool hasAttributeNS(string namespaceURI, string localName)
        {
            return ((IElement)_baseObject).HasAttribute(localName, namespaceURI == null ? "" : namespaceURI);
        }

        public string tagName
        {
            get { return ((IElement)_baseObject).Name; }
        }
    }

    #endregion

    #region Implementation - IJsText

    /// <summary>
    /// Implementation wrapper for IJsText
    /// </summary>
    public sealed class JsText : JsCharacterData, IJsText
    {
        public JsText(IText baseObject, ISvgScriptEngine engine) : base(baseObject, engine) { }

        public IJsText splitText(ulong offset)
        {
            var result = ((IText)_baseObject).SplitText((int)offset);
            return (result != null) ? CreateWrapper<IJsText>(result, _engine) : null;
        }
    }

    #endregion

    #region Implementation - IJsComment

    /// <summary>
    /// Implementation wrapper for IJsComment
    /// </summary>
    public sealed class JsComment : JsCharacterData, IJsComment
    {
        public JsComment(IComment baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }
    }

    #endregion

    #region Implementation - IJsCDataSection

    /// <summary>
    /// Implementation wrapper for IJsCDataSection
    /// </summary>
    public sealed class JsCDataSection : JsCharacterData, IJsCDataSection
    {
        public JsCDataSection(ICDataSection baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }
    }

    #endregion

    #region Implementation - IJsDocumentType

    /// <summary>
    /// Implementation wrapper for IJsDocumentType
    /// </summary>
    public sealed class JsDocumentType : JsNode, IJsDocumentType
    {
        public JsDocumentType(IDocumentType baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public string name
        {
            get { return _baseObject.Name; }
        }

        public IJsNamedNodeMap entities
        {
            get { var result = ((IDocumentType)_baseObject).Entities;
                return (result != null) ? CreateWrapper<IJsNamedNodeMap>(result, _engine) : null; }
        }

        public IJsNamedNodeMap notations
        {
            get { var result = ((IDocumentType)_baseObject).Notations;
                return (result != null) ? CreateWrapper<IJsNamedNodeMap>(result, _engine) : null; }
        }

        public string publicId
        {
            get { return ((IDocumentType)_baseObject).PublicId; }
        }

        public string systemId
        {
            get { return ((IDocumentType)_baseObject).SystemId; }
        }

        public string internalSubset
        {
            get { return ((IDocumentType)_baseObject).InternalSubset; }
        }
    }

    #endregion

    #region Implementation - IJsNotation: TODO

    /// <summary>
    /// Implementation wrapper for IJsNotation
    /// </summary>
    public sealed class JsNotation : JsNode, IJsNotation
    {
        public JsNotation(INotation baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public string publicId
        {
            get { return ((XmlNotation)_baseObject).PublicId; }
        }

        public string systemId
        {
            get { return ((XmlNotation)_baseObject).SystemId; }
        }
    }

    #endregion

    #region Implementation - IJsEntity: TODO

    /// <summary>
    /// Implementation wrapper for IJsEntity
    /// </summary>
    public sealed class JsEntity : JsNode, IJsEntity
    {
        public JsEntity(IEntity baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public string publicId
        {
            get { return ((XmlEntity)_baseObject).PublicId; }
        }

        public string systemId
        {
            get { return ((XmlEntity)_baseObject).SystemId; }
        }

        public string notationName
        {
            get { return ((XmlEntity)_baseObject).NotationName; }
        }
    }

    #endregion

    #region Implementation - IJsEntityReference

    /// <summary>
    /// Implementation wrapper for IJsEntityReference
    /// </summary>
    public sealed class JsEntityReference : JsNode, IJsEntityReference
    {
        public JsEntityReference(IEntityReference baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }
    }

    #endregion

    #region Implementation - IJsProcessingInstruction: TODO

    /// <summary>
    /// Implementation wrapper for IJsProcessingInstruction
    /// </summary>
    public sealed class JsProcessingInstruction : JsNode, IJsProcessingInstruction
    {
        public JsProcessingInstruction(IProcessingInstruction baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public string target
        {
            get { return ((XmlProcessingInstruction)_baseObject).Target; }
        }

        public string data
        {
            get { return ((XmlProcessingInstruction)_baseObject).Data; }
            set { ((XmlProcessingInstruction)_baseObject).Data = value; }
        }
    }

    #endregion

    #region Implementation - IJsDocumentFragment: TODO

    /// <summary>
    /// Implementation wrapper for IJsDocumentFragment
    /// </summary>
    public sealed class JsDocumentFragment : JsNode, IJsDocumentFragment
    {
        public JsDocumentFragment(IDocumentFragment baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }
    }

    #endregion

    #region Implementation - IJsDocument

    /// <summary>
    /// Implementation wrapper for IJsDocument
    /// </summary>
    public class JsDocument : JsNode, IJsDocument
    {
        public JsDocument(IDocument baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsElement createElement(string tagName)
        {
            var result = ((IDocument)_baseObject).CreateElement(tagName);
            return (result != null) ? CreateWrapper<IJsElement>(result, _engine) : null;
        }

        public IJsDocumentFragment createDocumentFragment()
        {
            var result = ((IDocument)_baseObject).CreateDocumentFragment();
            return (result != null) ? CreateWrapper<IJsDocumentFragment>(result, _engine) : null;
        }

        public IJsText createTextNode(string data)
        {
            var result = ((IDocument)_baseObject).CreateTextNode(data);
            return (result != null) ? CreateWrapper<IJsText>(result, _engine) : null;
        }

        public IJsComment createComment(string data)
        {
            var result = ((IDocument)_baseObject).CreateComment(data);
            return (result != null) ? CreateWrapper<IJsComment>(result, _engine) : null;
        }

        public IJsCDataSection createCDATASection(string data)
        {
            var result = ((IDocument)_baseObject).CreateCDataSection(data);
            return (result != null) ? CreateWrapper<IJsCDataSection>(result, _engine) : null;
        }

        public IJsProcessingInstruction createProcessingInstruction(string target, string data)
        {
            var result = ((IDocument)_baseObject).CreateProcessingInstruction(target, data);
            return (result != null) ? CreateWrapper<IJsProcessingInstruction>(result, _engine) : null;
        }

        public IJsAttr createAttribute(string name)
        {
            var result = ((IDocument)_baseObject).CreateAttribute(name);
            return (result != null) ? CreateWrapper<IJsAttr>(result, _engine) : null;
        }

        public IJsEntityReference createEntityReference(string name)
        {
            var result = ((IDocument)_baseObject).CreateEntityReference(name);
            return (result != null) ? CreateWrapper<IJsEntityReference>(result, _engine) : null;
        }

        public IJsNodeList getElementsByTagName(string tagname)
        {
            var result = ((IDocument)_baseObject).GetElementsByTagName(tagname);
            return (result != null) ? CreateWrapper<IJsNodeList>(result, _engine) : null;
        }

        public IJsNode importNode(IJsNode importedNode, bool deep)
        {
            var result = ((IDocument)_baseObject).ImportNode(importedNode.BaseObject, deep);
            return (result != null) ? CreateWrapper<IJsNode>(result, _engine) : null;
        }

        public IJsElement createElementNS(string namespaceURI, string qualifiedName)
        {
            var result = ((IDocument)_baseObject).CreateElementNs(namespaceURI == null ? "" : namespaceURI, qualifiedName);
            return (result != null) ? CreateWrapper<IJsElement>(result, _engine) : null;
        }

        public IJsAttr createAttributeNS(string namespaceURI, string qualifiedName)
        {
            var result = ((IDocument)_baseObject).CreateAttributeNs(namespaceURI == null ? "" : namespaceURI, qualifiedName);
            return (result != null) ? CreateWrapper<IJsAttr>(result, _engine) : null;
        }

        public IJsNodeList getElementsByTagNameNS(string namespaceURI, string localName)
        {
            var result = ((IDocument)_baseObject).GetElementsByTagNameNs(namespaceURI == null ? "" : namespaceURI, localName);
            return (result != null) ? CreateWrapper<IJsNodeList>(result, _engine) : null;
        }

        public IJsElement getElementById(string elementId)
        {
            var result = ((IDocument)_baseObject).GetElementById(elementId);
            return (result != null) ? CreateWrapper<IJsElement>(result, _engine) : null;
        }

        public IJsDocumentType doctype
        {
            get {
                var result = ((IDocument)_baseObject).Doctype;
                return (result != null) ? CreateWrapper<IJsDocumentType>(result, _engine) : null;
            }
        }

        public IJsDomImplementation implementation
        {
            get {
                var result = ((IDocument)_baseObject).Implementation;
                return (result != null) ? CreateWrapper<IJsDomImplementation>(result, _engine) : null;
            }
        }

        public IJsElement documentElement
        {
            get {
                var result = ((IDocument)_baseObject).DocumentElement;
                return (result != null) ? CreateWrapper<IJsElement>(result, _engine) : null;
            }
        }
    }    

    #endregion
}
