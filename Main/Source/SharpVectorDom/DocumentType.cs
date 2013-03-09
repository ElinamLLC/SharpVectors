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
		
		private EventTarget eventTarget;
		
		#endregion
		
		#region Constructors
		
		public DocumentType(string name, string publicId, string systemId,
            string internalSubset, Document document)
			: base(name, publicId, systemId, internalSubset, document)
		{
			eventTarget = new EventTarget(this);
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
		
		#region NON-DOM
		
		void IEventTargetSupport.FireEvent(
			IEvent @event)
		{
			eventTarget.FireEvent(@event);
		}
		
		#endregion
	}
}
