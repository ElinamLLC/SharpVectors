using System;
using System.Xml;

using SharpVectors.Dom.Events;

namespace SharpVectors.Dom
{
    /// <summary>
    /// Summary description for Element.
    /// </summary>
    public class Element : XmlElement, INode, IEventTargetSupport
    {
        #region Private Fields
        
        private EventTarget _eventTarget;
        
        #endregion
        
        #region Constructors
        
        public Element(string prefix, string localName, string namespaceUri,
            Document document) : base(prefix, localName, namespaceUri, document)
        {
            _eventTarget = new EventTarget(this);
        }
        
        #endregion
        
        #region IEventTarget interface
        
        #region DOM Level 2
        
        void IEventTarget.AddEventListener(string type, EventListener listener, bool useCapture)
        {
            _eventTarget.AddEventListener(type, listener, useCapture);
        }
        
        void IEventTarget.RemoveEventListener(string type, EventListener listener, bool useCapture)
        {
            _eventTarget.RemoveEventListener(type, listener, useCapture);
        }

        bool IEventTarget.DispatchEvent(IEvent evt)
        {
            return _eventTarget.DispatchEvent(evt);
        }
        
        #endregion
        
        #region DOM Level 3 Experimental
        
        void IEventTarget.AddEventListenerNs(string namespaceUri, string type, EventListener listener,
            bool useCapture, object eventGroup)
        {
            _eventTarget.AddEventListenerNs(namespaceUri, type, listener, useCapture, eventGroup);
        }
        
        void IEventTarget.RemoveEventListenerNs(string namespaceUri, string type, 
            EventListener listener, bool useCapture)
        {
            _eventTarget.RemoveEventListenerNs(namespaceUri, type, listener, useCapture);
        }
        
        bool IEventTarget.WillTriggerNs(string namespaceUri, string type)
        {
            return _eventTarget.WillTriggerNs(namespaceUri, type);
        }
        
        bool IEventTarget.HasEventListenerNs(string namespaceUri, string type)
        {
            return _eventTarget.HasEventListenerNs(namespaceUri, type);
        }
        
        #endregion
        
        #endregion
        
        #region NON-DOM
        
        void IEventTargetSupport.FireEvent(IEvent evt)
        {
            _eventTarget.FireEvent(evt);
        }
        
        #endregion
    }
}
