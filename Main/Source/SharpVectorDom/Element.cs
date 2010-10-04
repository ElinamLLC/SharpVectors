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
		
		private EventTarget eventTarget;
		
		#endregion
		
		#region Constructors
		
		public Element(string prefix, string localName, string namespaceUri,
			Document document) : base(prefix, localName, namespaceUri, document)
		{
			eventTarget = new EventTarget(this);
		}
		
		#endregion
		
		#region IEventTarget interface
		
		#region Methods
		
		#region DOM Level 2
		
		void IEventTarget.AddEventListener(string type, EventListener listener,
			bool useCapture)
		{
			eventTarget.AddEventListener(type, listener, useCapture);
		}
		
		void IEventTarget.RemoveEventListener(string type, EventListener listener,
			bool useCapture)
		{
			eventTarget.RemoveEventListener(type, listener, useCapture);
		}

        bool IEventTarget.DispatchEvent(IEvent evt)
		{
            return eventTarget.DispatchEvent(evt);
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
		
		void IEventTargetSupport.FireEvent(IEvent evt)
		{
            eventTarget.FireEvent(evt);
		}
		
		#endregion
	}
}
