using System;
using System.Xml;

using SharpVectors.Dom.Events;

namespace SharpVectors.Dom
{
    /// <summary>
    /// Summary description for Document.
    /// </summary>
    public class Document : XmlDocument, IDocument, INode, IEventTargetSupport, IDocumentEvent
    {
        #region Private Fields

        private EventTarget eventTarget;
        private bool mutationEvents = false;

        #endregion

        #region Constructors

        public Document()
        {
            eventTarget = new EventTarget(this);

            NodeChanged += new XmlNodeChangedEventHandler(WhenNodeChanged);
            NodeChanging += new XmlNodeChangedEventHandler(WhenNodeChanging);
            NodeInserted += new XmlNodeChangedEventHandler(WhenNodeInserted);
            NodeInserting += new XmlNodeChangedEventHandler(WhenNodeInserting);
            NodeRemoved += new XmlNodeChangedEventHandler(WhenNodeRemoved);
            NodeRemoving += new XmlNodeChangedEventHandler(WhenNodeRemoving);
        }

        protected internal Document(
            DomImplementation domImplementation)
            : base(domImplementation)
        {
            eventTarget = new EventTarget(this);
        }

        public Document(
            XmlNameTable nameTable)
            : base(nameTable)
        {
            eventTarget = new EventTarget(this);
        }

        #endregion

        #region Document interface

        #region Configuration Properties

        /// <summary>
        /// Enables or disables mutation events.
        /// </summary>
        public bool MutationEvents
        {
            get
            {
                return mutationEvents;
            }
            set
            {
                mutationEvents = value;
            }
        }

        #endregion

        #region System.Xml events to Dom events

        private void WhenNodeChanged(
            object sender,
            XmlNodeChangedEventArgs e)
        {
            if (mutationEvents)
            {
                throw new NotImplementedException();
            }

        }

        private void WhenNodeChanging(
            object sender,
            XmlNodeChangedEventArgs e)
        {
            // Cannot perform ReplaceText/DeleteText/InsertText here because
            // System.Xml events do not provide enough information.
            if (mutationEvents)
            {
                throw new NotImplementedException();
            }

        }

        private void WhenNodeInserted(
            object sender,
            XmlNodeChangedEventArgs e)
        {
            INode newParent = e.NewParent as INode;
            INode node = e.Node as INode;

            if (newParent != null && node != null)
            {
                InsertedNode(newParent, node, false);
            }
        }

        private void WhenNodeInserting(
            object sender,
            XmlNodeChangedEventArgs e)
        {
            INode node = e.Node as INode;

            if (node != null)
            {
                InsertingNode(node, false);
            }
        }

        private void WhenNodeRemoved(
            object sender,
            XmlNodeChangedEventArgs e)
        {
            INode node = e.Node as INode;

            if (node != null)
            {
                RemovedNode(node, false);
            }
        }

        private void WhenNodeRemoving(
            object sender,
            XmlNodeChangedEventArgs e)
        {
            INode oldParent = e.OldParent as INode;
            INode node = e.NewParent as INode;

            if (oldParent != null && node != null)
            {
                RemovingNode(oldParent, node, false);
            }
        }

        #endregion

        #region Notification Methods

        /// <summary>
        /// A method to be called when some text was changed in a text node,
        /// so that live objects can be notified.
        /// </summary>
        /// <param name="node">
        /// </param>
        protected internal virtual void ReplacedText(
            INode node)
        {
            if (mutationEvents)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// A method to be called when some text was deleted from a text node,
        /// so that live objects can be notified.
        /// </summary>
        /// <param name="node">
        /// </param>
        /// <param name="offset">
        /// </param>
        /// <param name="count">
        /// </param>
        protected internal virtual void DeletedText(
            INode node,
            int offset,
            int count)
        {
            if (mutationEvents)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// A method to be called when some text was inserted into a text node,
        /// so that live objects can be notified.
        /// </summary>
        /// <param name="node">
        /// </param>
        /// <param name="offset">
        /// </param>
        /// <param name="count">
        /// </param>
        protected internal virtual void InsertedText(
            INode node,
            int offset,
            int count)
        {
            if (mutationEvents)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// A method to be called when a character data node has been modified.
        /// </summary>
        /// <param name="node">
        /// </param>
        protected internal virtual void ModifyingCharacterData(
            INode node)
        {
            if (mutationEvents)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// A method to be called when a character data node has been modified.
        /// </summary>
        /// <param name="node">
        /// </param>
        /// <param name="oldvalue">
        /// </param>
        /// <param name="value">
        /// </param>
        protected internal virtual void ModifiedCharacterData(
            INode node,
            string oldvalue,
            string value)
        {
            if (mutationEvents)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// A method to be called when a node is about to be inserted in the tree.
        /// </summary>
        /// <param name="node">
        /// </param>
        /// <param name="replace">
        /// </param>
        protected internal virtual void InsertingNode(
            INode node,
            bool replace)
        {
            if (mutationEvents)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// A method to be called when a node has been inserted in the tree.
        /// </summary>
        /// <param name="node">
        /// </param>
        /// <param name="newInternal">
        /// </param>
        /// <param name="replace">
        /// </param>
        protected internal virtual void InsertedNode(
            INode node,
            INode newInternal,
            bool replace)
        {
            if (mutationEvents)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// A method to be called when a node is about to be removed from the tree.
        /// </summary>
        /// <param name="node">
        /// </param>
        /// <param name="oldChild">
        /// </param>
        /// <param name="replace">
        /// </param>
        protected internal virtual void RemovingNode(
            INode node,
            INode oldChild,
            bool replace)
        {
            if (mutationEvents)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// A method to be called when a node has been removed from the tree.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="replace"></param>
        protected internal virtual void RemovedNode(
            INode node,
            bool replace)
        {
            if (mutationEvents)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// A method to be called when a node is about to be replaced in the tree.
        /// </summary>
        /// <param name="node">
        /// </param>
        protected internal virtual void replacingNode(
            INode node)
        {
            if (mutationEvents)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// A method to be called when a node has been replaced in the tree.
        /// </summary>
        /// <param name="node">
        /// </param>
        protected internal virtual void ReplacedNode(
            INode node)
        {
            if (mutationEvents)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// A method to be called when an attribute value has been modified.
        /// </summary>
        /// <param name="attr">
        /// </param>
        /// <param name="oldvalue">
        /// </param>
        protected internal virtual void ModifiedAttrValue(
            IAttribute attr,
            string oldvalue)
        {
            if (mutationEvents)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// A method to be called when an attribute node has been set.
        /// </summary>
        /// <param name="attribute">
        /// </param>
        /// <param name="previous">
        /// </param>
        protected internal virtual void SetAttrNode(
            IAttribute attribute,
            IAttribute previous)
        {
            if (mutationEvents)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// A method to be called when an attribute node has been removed.
        /// </summary>
        /// <param name="attribute">
        /// </param>
        /// <param name="oldOwner">
        /// </param>
        /// <param name="name">
        /// </param>
        protected internal virtual void RemovedAttrNode(
            IAttribute attribute,
            INode oldOwner,
            string name)
        {
            if (mutationEvents)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// A method to be called when an attribute node has been renamed.
        /// </summary>
        /// <param name="oldAttribute">
        /// </param>
        /// <param name="newAttribute">
        /// </param>
        protected internal virtual void RenamedAttrNode(
            IAttribute oldAttribute,
            IAttribute newAttribute)
        {
            if (mutationEvents)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// A method to be called when an element has been renamed.
        /// </summary>
        /// <param name="oldElement">
        /// </param>
        /// <param name="newElement">
        /// </param>
        protected internal virtual void RenamedElement(
            IElement oldElement,
            IElement newElement)
        {
            if (mutationEvents)
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #endregion

        #region XmlDocument interface

        public override XmlAttribute CreateAttribute(
            string prefix,
            string localName,
            string namespaceUri)
        {
            return new Attribute(prefix, localName, namespaceUri, this);
        }

        public override XmlCDataSection CreateCDataSection(
            string data)
        {
            return new CDataSection(data, this);
        }

        public override XmlComment CreateComment(
            string data)
        {
            return new Comment(data, this);
        }

        public override XmlDocumentFragment CreateDocumentFragment()
        {
            return new DocumentFragment(this);
        }

        public override XmlDocumentType CreateDocumentType(
            string name,
            string publicId,
            string systemId,
            string internalSubset)
        {
            return new DocumentType(name, publicId, systemId, internalSubset, this);
        }

        public override XmlElement CreateElement(
            string prefix,
            string localName,
            string namespaceUri)
        {
            return new Element(prefix, localName, namespaceUri, this);
        }

        public override XmlEntityReference CreateEntityReference(
            string name)
        {
            return new EntityReference(name, this);
        }

        public override XmlProcessingInstruction CreateProcessingInstruction(
            string target,
            string data)
        {
            return new ProcessingInstruction(target, data, this);
        }

        public override XmlSignificantWhitespace CreateSignificantWhitespace(
            string text)
        {
            return new SignificantWhitespace(text, this);
        }

        public override XmlText CreateTextNode(
            string text)
        {
            return new Text(text, this);
        }

        public override XmlWhitespace CreateWhitespace(
            string text)
        {
            return new Whitespace(text, this);
        }

        public override XmlDeclaration CreateXmlDeclaration(
            string version,
            string encoding,
            string standalone)
        {
            return new Declaration(version, encoding, standalone, this);
        }

        #endregion

        #region IEventTarget interface

        #region Methods

        #region DOM Level 2

        void IEventTarget.AddEventListener(
            string type,
            EventListener listener,
            bool useCapture)
        {
            eventTarget.AddEventListener(type, listener, useCapture);
        }

        void IEventTarget.RemoveEventListener(
            string type,
            EventListener listener,
            bool useCapture)
        {
            eventTarget.RemoveEventListener(type, listener, useCapture);
        }

        bool IEventTarget.DispatchEvent(
            IEvent @event)
        {
            return eventTarget.DispatchEvent(@event);
        }

        #endregion

        #region DOM Level 3 Experimental

        void IEventTarget.AddEventListenerNs(
            string namespaceUri,
            string type,
            EventListener listener,
            bool useCapture,
            object eventGroup)
        {
            eventTarget.AddEventListenerNs(namespaceUri, type, listener, useCapture, eventGroup);
        }

        void IEventTarget.RemoveEventListenerNs(
            string namespaceUri,
            string type,
            EventListener listener,
            bool useCapture)
        {
            eventTarget.RemoveEventListenerNs(namespaceUri, type, listener, useCapture);
        }

        bool IEventTarget.WillTriggerNs(
            string namespaceUri,
            string type)
        {
            return eventTarget.WillTriggerNs(namespaceUri, type);
        }

        bool IEventTarget.HasEventListenerNs(
            string namespaceUri,
            string type)
        {
            return eventTarget.HasEventListenerNs(namespaceUri, type);
        }

        #endregion

        #endregion

        #endregion

        #region IDocument interface

        IDocumentType IDocument.Doctype
        {
            get
            {
                return (IDocumentType)DocumentType;
            }
        }

        IDomImplementation IDocument.Implementation
        {
            get
            {
                throw new DomException(DomExceptionType.NotSupportedErr);
            }
        }

        IElement IDocument.DocumentElement
        {
            get
            {
                return (IElement)DocumentElement;
            }
        }

        IElement IDocument.CreateElement(
            string tagName)
        {
            return (IElement)CreateElement(tagName);
        }

        IDocumentFragment IDocument.CreateDocumentFragment()
        {
            return (IDocumentFragment)CreateDocumentFragment();
        }

        IText IDocument.CreateTextNode(
            string data)
        {
            return (IText)CreateTextNode(data);
        }

        IComment IDocument.CreateComment(
            string data)
        {
            return (IComment)CreateComment(data);
        }

        ICDataSection IDocument.CreateCDataSection(
            string data)
        {
            return (ICDataSection)CreateCDataSection(data);
        }

        IProcessingInstruction IDocument.CreateProcessingInstruction(
            string target,
            string data)
        {
            return (IProcessingInstruction)CreateProcessingInstruction(target, data);
        }

        IAttribute IDocument.CreateAttribute(
            string name)
        {
            return (IAttribute)CreateAttribute(name);
        }

        IEntityReference IDocument.CreateEntityReference(
            string name)
        {
            return (IEntityReference)CreateEntityReference(name);
        }

        INodeList IDocument.GetElementsByTagName(
            string tagname)
        {
            return new NodeListAdapter(GetElementsByTagName(tagname));
        }

        INode IDocument.ImportNode(
            INode importedNode,
            bool deep)
        {
            return (INode)ImportNode((XmlNode)importedNode, deep);
        }

        IElement IDocument.CreateElementNs(
            string namespaceUri,
            string qualifiedName)
        {
            return (IElement)CreateElement(qualifiedName, namespaceUri);
        }

        IAttribute IDocument.CreateAttributeNs(
            string namespaceUri,
            string qualifiedName)
        {
            return (IAttribute)CreateAttribute(qualifiedName, namespaceUri);
        }

        INodeList IDocument.GetElementsByTagNameNs(
            string namespaceUri,
            string localName)
        {
            return new NodeListAdapter(GetElementsByTagName(localName, namespaceUri));
        }

        IElement IDocument.GetElementById(
            string elementId)
        {
            object res = GetElementById(elementId);
            if (res != null)
                return (IElement)res;
            else
                return null;
        }

        #endregion

        #region IDocumentEvent interface

        #region DOM Level 2

        public virtual IEvent CreateEvent(
            string eventType)
        {
            switch (eventType)
            {
                case "Event":
                case "Events":
                case "HTMLEvents":
                    return new Event();
                case "MutationEvent":
                case "MutationEvents":
                    return new MutationEvent();
                case "UIEvent":
                case "UIEvents":
                    return new UiEvent();
                case "MouseEvent":
                case "MouseEvents":
                    return new MouseEvent();
                default:
                    throw new DomException(DomExceptionType.NotSupportedErr,
                        "Event type \"" + eventType + "\" not suppoted");
            }
        }

        #endregion

        #region DOM Level 3 Experimental

        public virtual bool CanDispatch(string namespaceUri, string type)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region NON-DOM

        void IEventTargetSupport.FireEvent(
            IEvent @event)
        {
            eventTarget.FireEvent(@event);
        }

        #endregion
    }
}
