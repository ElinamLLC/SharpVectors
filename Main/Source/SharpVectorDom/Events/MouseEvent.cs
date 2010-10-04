using System;

using SharpVectors.Dom.Views;

namespace SharpVectors.Dom.Events
{
	/// <summary>
	/// Summary description for MouseEvent.
	/// </summary>
	public class MouseEvent : UiEvent, IMouseEvent
	{
		#region Private Fields
		
		private long screenX;
		private long screeny;
		private long clientX;
		private long clientY;
		private bool crtlKey;
		private bool shiftKey;
		private bool altKey;
		private bool metaKey;
		private ushort button;
		private IEventTarget relatedTarget;
		private bool altGraphKey;
		
		#endregion
		
		#region Constructors
		
		public MouseEvent()
		{
		}
		
		public MouseEvent(
			string eventType,
			bool bubbles,
			bool cancelable,
			IAbstractView view,
			long detail,
			long screenX,
			long screenY,
			long clientX,
			long clientY,
			bool ctrlKey,
			bool altKey,
			bool shiftKey,
			bool metaKey,
			ushort button,
			IEventTarget relatedTarget)
		{
			InitMouseEvent(
				eventType, bubbles, cancelable, view, detail,
				screenX, screenY, clientX, clientY,
				ctrlKey, altKey, shiftKey, metaKey, button,
				relatedTarget);
		}
		
		public MouseEvent(
			string namespaceUri,
			string eventType,
			bool bubbles,
			bool cancelable,
			IAbstractView view,
			long detail,
			long screenX,
			long screenY,
			long clientX,
			long clientY,
			bool ctrlKey,
			bool altKey,
			bool shiftKey,
			bool metaKey,
			ushort button,
			IEventTarget relatedTarget,
			bool altGraphKey)
		{
			InitMouseEventNs(
				namespaceUri, eventType, bubbles, cancelable, view, detail,
				screenX, screenY, clientX, clientY,
				ctrlKey, altKey, shiftKey, metaKey, button,
				relatedTarget, altGraphKey);
		}
		
		public MouseEvent(
			EventType eventType,
			bool bubbles,
			bool cancelable,
			IAbstractView view,
			long detail,
			long screenX,
			long screenY,
			long clientX,
			long clientY,
			bool ctrlKey,
			bool altKey,
			bool shiftKey,
			bool metaKey,
			ushort button,
			IEventTarget relatedTarget,
			bool altGraphKey)
		{
			InitMouseEventNs(
				eventType.NamespaceUri, eventType.Name,
				bubbles, cancelable, view, detail,
				screenX, screenY, clientX, clientY,
				ctrlKey, altKey, shiftKey, metaKey, button,
				relatedTarget, altGraphKey);
		}
		
		#endregion
		
		#region IMouseEvent interface
		
		#region Public Properties
		
		public long ScreenX
		{
			get
			{
				return screenX;
			}
		}
		
		public long ScreenY
		{
			get
			{
				return screeny;
			}
		}
		
		public long ClientX
		{
			get
			{
				return clientX;
			}
		}
		
		public long ClientY
		{
			get
			{
				return clientY;
			}
		}
		
		public bool CtrlKey
		{
			get
			{
				return crtlKey;
			}
		}
		
		public bool ShiftKey
		{
			get
			{
				return shiftKey;
			}
		}
		
		public bool AltKey
		{
			get
			{
				return altKey;
			}
		}
		
		public bool MetaKey
		{
			get
			{
				return metaKey;
			}
		}
		
		public ushort Button
		{
			get
			{
				return button;
			}
		}
		
		public IEventTarget RelatedTarget
		{
			get
			{
				return relatedTarget;
			}
		}
		
		public bool AltGraphKey
		{
			get
			{
				return altGraphKey;
			}
		}
		
		#endregion
		
		#region Public Methods
		
		public void InitMouseEvent(
			string eventType,
			bool bubbles,
			bool cancelable,
			IAbstractView view,
			long detail,
			long screenX,
			long screenY,
			long clientX,
			long clientY,
			bool ctrlKey,
			bool altKey,
			bool shiftKey,
			bool metaKey,
			ushort button,
			IEventTarget relatedTarget)
		{
			InitUiEvent(eventType, bubbles, cancelable, view, detail);
			
			this.screenX  = screenX;
			this.screeny  = screenY;
			this.clientX  = clientX;
			this.clientY  = clientY;
			this.crtlKey  = ctrlKey;
			this.shiftKey = shiftKey;
			this.altKey   = altKey;
			this.metaKey  = metaKey;
			this.button   = button;
			this.relatedTarget = relatedTarget;
			//this.altGraphKey   = altGraphKey;
		}
		
		public void InitMouseEventNs(
			string namespaceUri,
			string eventType,
			bool bubbles,
			bool cancelable,
			IAbstractView view,
			long detail,
			long screenX,
			long screenY,
			long clientX,
			long clientY,
			bool ctrlKey,
			bool altKey,
			bool shiftKey,
			bool metaKey,
			ushort button,
			IEventTarget relatedTarget,
			bool altGraphKey)
		{
			InitUiEventNs(namespaceUri, eventType, bubbles, cancelable, view, detail);
			
			this.screenX = screenX;
			this.screeny = screenY;
			this.clientX = clientX;
			this.clientY = clientY;
			this.crtlKey = ctrlKey;
			this.shiftKey = shiftKey;
			this.altKey = altKey;
			this.metaKey = metaKey;
			this.button = button;
			this.relatedTarget = relatedTarget;
			this.altGraphKey = altGraphKey;
		}
		
		#endregion
		
		#endregion
	}
}
