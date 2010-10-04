using System;

namespace SharpVectors.Dom.Events
{
	/// <summary>
	/// Summary description for Event.
	/// </summary>
	public class Event : IEvent
	{
		#region Private Fields
		
		internal bool stopped = false;
		internal IEventTarget eventTarget = null;
		internal IEventTarget currentTarget = null;
		internal EventPhase eventPhase = EventPhase.AtTarget;
		
		private bool bubbles;
		private DateTime timeStamp = DateTime.Now;
		private bool cancelable;
		private string namespaceUri;
		protected string eventType;
		
		#endregion
		
		#region Constructors
		
		public Event()
		{
		}
		
		public Event(
			string eventType,
			bool bubbles,
			bool cancelable)
		{
			InitEvent(eventType, bubbles, cancelable);
		}
		
		public Event(
			string namespaceUri,
			string eventType,
			bool bubbles,
			bool cancelable)
		{
			InitEventNs(namespaceUri, eventType, bubbles, cancelable);
		}
		
		#endregion
		
		#region Properties
		
		#region DOM Level 2
		
		public string Type
		{
			get
			{
				return eventType;
			}
		}
		
		public IEventTarget Target
		{
			get
			{
				return eventTarget;
			}
		}
		
		public IEventTarget CurrentTarget
		{
			get
			{
				return currentTarget;
			}
		}
		
		public EventPhase EventPhase
		{
			get
			{
				return eventPhase;
			}
		}
		
		public bool Bubbles
		{
			get
			{
				return bubbles;
			}
		}
		
		public bool Cancelable
		{
			get
			{
				return cancelable;
			}
		}
		
		public DateTime TimeStamp
		{
			get
			{
				return timeStamp;
			}
		}
		
		#endregion
		
		#region DOM Level 3 Experimental
		
		public string NamespaceUri
		{
			get
			{
				return namespaceUri;
			}
		}
		
		public bool IsCustom
		{
			get
			{
				throw new NotImplementedException();
			}
		}
		
		public bool IsDefaultPrevented
		{
			get
			{
				throw new NotImplementedException();
			}
		}
		
		#endregion
		
		#endregion
		
		#region Methods
		
		#region DOM Level 2
		
		public void StopPropagation()
		{
			throw new NotImplementedException();
		}
		
		public void PreventDefault()
		{
			throw new NotImplementedException();
		}
		
		public void InitEvent(
			string eventType,
			bool bubbles,
			bool cancelable)
		{
			this.namespaceUri = null;
			this.eventType = eventType;
			this.bubbles = bubbles;
			this.cancelable = cancelable;
		}
		
		#endregion
		
		#region DOM Level 3 Experimental
		
		public void InitEventNs(
			string namespaceUri,
			string eventType,
			bool bubbles,
			bool cancelable)
		{
			this.namespaceUri = namespaceUri == "" ? null : namespaceUri;
			this.eventType = eventType;
			this.bubbles = bubbles;
			this.cancelable = cancelable;
		}
		
		public void StopImmediatePropagation()
		{
			throw new NotImplementedException();
		}
		
		#endregion
		
		#endregion
	}
}
