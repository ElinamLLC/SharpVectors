using System;
using System.Xml;

using SharpVectors.Dom.Events;

namespace SharpVectors.Dom
{
	/// <summary>
	/// Summary description for DocumentType.
	/// </summary>
	public class DocumentType : XmlDocumentType, INode, IEventTargetSupport
	{
		#region Private Fields
		
		private EventTarget _eventTarget;
		
		#endregion
		
		#region Constructors
		
		public DocumentType(string name, string publicId, string systemId, string internalSubset, Document document)
			: base(name, publicId, systemId, internalSubset, document)
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
		
		bool IEventTarget.DispatchEvent(IEvent eventArgs)
		{
			return _eventTarget.DispatchEvent(eventArgs);
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
		
		void IEventTargetSupport.FireEvent(IEvent eventArgs)
		{
			_eventTarget.FireEvent(eventArgs);
		}
		
		#endregion
	}
}
