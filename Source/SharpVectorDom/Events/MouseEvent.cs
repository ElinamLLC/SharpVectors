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
		
		private long _screenX;
		private long _screeny;
		private long _clientX;
		private long _clientY;
		private bool _crtlKey;
		private bool _shiftKey;
		private bool _altKey;
		private bool _metaKey;
		private ushort _button;
		private bool _altGraphKey;
		private IEventTarget _relatedTarget;
		
		#endregion
		
		#region Constructors
		
		public MouseEvent()
		{
		}
		
		public MouseEvent(string eventType, bool bubbles, bool cancelable, IAbstractView view,
			long detail, long screenX, long screenY, long clientX, long clientY, bool ctrlKey,
			bool altKey, bool shiftKey, bool metaKey, ushort button, IEventTarget relatedTarget)
		{
			InitMouseEvent(eventType, bubbles, cancelable, view, detail, screenX, screenY,
                clientX, clientY, ctrlKey, altKey, shiftKey, metaKey, button, relatedTarget);
		}
		
		public MouseEvent(string namespaceUri, string eventType, bool bubbles, bool cancelable,
			IAbstractView view, long detail, long screenX, long screenY, long clientX, long clientY,
			bool ctrlKey, bool altKey, bool shiftKey, bool metaKey, ushort button, 
            IEventTarget relatedTarget, bool altGraphKey)
		{
			InitMouseEventNs(namespaceUri, eventType, bubbles, cancelable, view, detail, screenX, screenY, 
                clientX, clientY, ctrlKey, altKey, shiftKey, metaKey, button, relatedTarget, altGraphKey);
		}
		
		public MouseEvent(EventType eventType, bool bubbles, bool cancelable, IAbstractView view,
			long detail, long screenX, long screenY, long clientX, long clientY, bool ctrlKey, bool altKey,
			bool shiftKey, bool metaKey, ushort button, IEventTarget relatedTarget, bool altGraphKey)
		{
			InitMouseEventNs(eventType.NamespaceUri, eventType.Name, bubbles, cancelable, view, detail,
				screenX, screenY, clientX, clientY, ctrlKey, altKey, shiftKey, metaKey, button,
				relatedTarget, altGraphKey);
		}
		
		#endregion
		
		#region IMouseEvent interface
		
		public long ScreenX
		{
			get
			{
				return _screenX;
			}
		}
		
		public long ScreenY
		{
			get
			{
				return _screeny;
			}
		}
		
		public long ClientX
		{
			get
			{
				return _clientX;
			}
		}
		
		public long ClientY
		{
			get
			{
				return _clientY;
			}
		}
		
		public bool CtrlKey
		{
			get
			{
				return _crtlKey;
			}
		}
		
		public bool ShiftKey
		{
			get
			{
				return _shiftKey;
			}
		}
		
		public bool AltKey
		{
			get
			{
				return _altKey;
			}
		}
		
		public bool MetaKey
		{
			get
			{
				return _metaKey;
			}
		}
		
		public ushort Button
		{
			get
			{
				return _button;
			}
		}
		
		public IEventTarget RelatedTarget
		{
			get
			{
				return _relatedTarget;
			}
		}
		
		public bool AltGraphKey
		{
			get
			{
				return _altGraphKey;
			}
		}
		
		public void InitMouseEvent(string eventType, bool bubbles, bool cancelable, IAbstractView view, 
            long detail, long screenX, long screenY, long clientX, long clientY, bool ctrlKey, 
            bool altKey, bool shiftKey, bool metaKey, ushort button, IEventTarget relatedTarget)
		{
			InitUiEvent(eventType, bubbles, cancelable, view, detail);
			
			_screenX       = screenX;
			_screeny       = screenY;
			_clientX       = clientX;
			_clientY       = clientY;
			_crtlKey       = ctrlKey;
			_shiftKey      = shiftKey;
			_altKey        = altKey;
			_metaKey       = metaKey;
			_button        = button;
			_relatedTarget = relatedTarget;
		}
		
		public void InitMouseEventNs(string namespaceUri, string eventType, bool bubbles, bool cancelable,
			IAbstractView view, long detail, long screenX, long screenY, long clientX, long clientY, bool ctrlKey, 
            bool altKey, bool shiftKey, bool metaKey, ushort button, IEventTarget relatedTarget, bool altGraphKey)
		{
			InitUiEventNs(namespaceUri, eventType, bubbles, cancelable, view, detail);
			
			_screenX       = screenX;
			_screeny       = screenY;
			_clientX       = clientX;
			_clientY       = clientY;
			_crtlKey       = ctrlKey;
			_shiftKey      = shiftKey;
			_altKey        = altKey;
			_metaKey       = metaKey;
			_button        = button;
			_relatedTarget = relatedTarget;
			_altGraphKey   = altGraphKey;
		}
		
		#endregion
	}
}
