using System;

using SharpVectors.Dom.Views;

namespace SharpVectors.Dom.Events
{
	/// <summary>
	/// Summary description for UiEvent.
	/// </summary>
	public class UiEvent: Event, IUiEvent
	{
		#region Private Fields
		
		private long _detail;
		private IAbstractView _view;
		
		#endregion
		
		#region Constructors
		
		public UiEvent()
		{
		}
		
		public UiEvent(string eventType, bool bubbles, bool cancelable, IAbstractView view, long detail)
		{
			InitUiEvent(eventType, bubbles, cancelable, view, detail);
		}
		
		public UiEvent(string namespaceUri, string eventType, bool bubbles,
			bool cancelable, IAbstractView view, long detail)
		{
			InitUiEventNs(namespaceUri, eventType, bubbles, cancelable, view, detail);
		}
		
		#endregion
		
		#region IUiEvent interface
		
		public IAbstractView View
		{
			get
			{
				return _view;
			}
		}
		
		public long Detail
		{
			get
			{
				return _detail;
			}
		}
		
		public void InitUiEvent(string eventType, bool bubbles, bool cancelable,
			IAbstractView view, long detail)
		{
			InitEvent(eventType, bubbles, cancelable);
			
			_view   = view;
			_detail = detail;
		}
		
		public void InitUiEventNs(string namespaceUri, string eventType, bool bubbles,
			bool cancelable, IAbstractView view, long detail)
		{
			InitEventNs(namespaceUri, eventType, bubbles, cancelable);
			
			_view   = view;
			_detail = detail;
		}
		
		#endregion
	}
}
