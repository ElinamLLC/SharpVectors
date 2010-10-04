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
		
		private IAbstractView view;
		private long detail;
		
		#endregion
		
		#region Constructors
		
		public UiEvent()
		{
		}
		
		public UiEvent(
			string eventType,
			bool bubbles,
			bool cancelable,
			IAbstractView view,
			long detail)
		{
			InitUiEvent(eventType, bubbles, cancelable, view, detail);
		}
		
		public UiEvent(
			string namespaceUri,
			string eventType,
			bool bubbles,
			bool cancelable,
			IAbstractView view,
			long detail)
		{
			InitUiEventNs(namespaceUri, eventType, bubbles, cancelable, view, detail);
		}
		
		#endregion
		
		#region IUiEvent interface
		
		public IAbstractView View
		{
			get
			{
				return view;
			}
		}
		
		public long Detail
		{
			get
			{
				return detail;
			}
		}
		
		public void InitUiEvent(
			string eventType,
			bool bubbles,
			bool cancelable,
			IAbstractView view,
			long detail)
		{
			InitEvent(eventType, bubbles, cancelable);
			
			this.view = view;
			this.detail = detail;
		}
		
		public void InitUiEventNs(
			string namespaceUri,
			string eventType,
			bool bubbles,
			bool cancelable,
			IAbstractView view,
			long detail)
		{
			InitEventNs(namespaceUri, eventType, bubbles, cancelable);
			
			this.view = view;
			this.detail = detail;
		}
		
		#endregion
	}
}
