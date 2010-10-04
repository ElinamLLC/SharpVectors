using System;

using SharpVectors.Dom.Views;

namespace SharpVectors.Dom.Events
{
	/// <summary>
	/// Summary description for KeyboardEvent.
	/// </summary>
	public class KeyboardEvent : UiEvent, IKeyboardEvent
	{
		#region Private Fields
		
		private string keyIdentifier;
		private KeyLocationCode keyLocation;
		private bool ctrlKey;
		private bool shiftKey;
		private bool altKey;
		private bool metaKey;
		private bool altGraphKey;
		
		#endregion
		
		#region Constructors
		
		public KeyboardEvent()
		{
		}
		
		public KeyboardEvent(
			string eventType,
			bool bubbles,
			bool cancelable,
			IAbstractView view,
			string keyIdentifier,
			KeyLocationCode keyLocation,
			bool ctrlKey,
			bool shiftKey,
			bool altKey,
			bool metaKey,
			bool altGraphKey)
		{
			InitKeyboardEvent(
				eventType, bubbles, cancelable, view,
				keyIdentifier, keyLocation,
				ctrlKey, shiftKey, altKey, metaKey, altGraphKey);
		}
		
		public KeyboardEvent(
			string namespaceUri,
			string eventType,
			bool bubbles,
			bool cancelable,
			IAbstractView view,
			string keyIdentifier,
			KeyLocationCode keyLocation,
			bool ctrlKey,
			bool shiftKey,
			bool altKey,
			bool metaKey,
			bool altGraphKey)
		{
			InitKeyboardEventNs(
				namespaceUri, eventType, bubbles, cancelable, view,
				keyIdentifier, keyLocation,
				ctrlKey, shiftKey, altKey, metaKey, altGraphKey);
		}
		
		#endregion
		
		#region IKeyboardEvent interface
		
		#region Public Properties
		
		public string KeyIdentifier
		{
			get
			{
				return keyIdentifier;
			}
		}
		
		public KeyLocationCode KeyLocation
		{
			get
			{
				return keyLocation;
			}
		}
		
		public bool CtrlKey
		{
			get
			{
				return ctrlKey;
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
		
		public bool AltGraphKey
		{
			get
			{
				return altGraphKey;
			}
		}
		
		#endregion
		
		#region Public Methods
		
		public void InitKeyboardEvent(
			string eventType,
			bool bubbles,
			bool cancelable,
			IAbstractView view,
			string keyIdentifier,
			KeyLocationCode keyLocation,
			bool ctrlKey,
			bool shiftKey,
			bool altKey,
			bool metaKey,
			bool altGraphKey)
		{
			InitUiEvent(eventType, bubbles, cancelable, view, 0);
			
			this.keyIdentifier = keyIdentifier;
			this.keyLocation = keyLocation;
			this.ctrlKey = ctrlKey;
			this.shiftKey = shiftKey;
			this.altKey = altKey;
			this.metaKey = metaKey;
			this.altGraphKey = altGraphKey;
		}
		
		public void InitKeyboardEventNs(
			string namespaceUri,
			string eventType,
			bool bubbles,
			bool cancelable,
			IAbstractView view,
			string keyIdentifier,
			KeyLocationCode keyLocation,
			bool ctrlKey,
			bool shiftKey,
			bool altKey,
			bool metaKey,
			bool altGraphKey)
		{
			InitUiEventNs(namespaceUri, eventType, bubbles, cancelable, view, 0);
			
			this.keyIdentifier = keyIdentifier;
			this.keyLocation = keyLocation;
			this.ctrlKey = ctrlKey;
			this.shiftKey = shiftKey;
			this.altKey = altKey;
			this.metaKey = metaKey;
			this.altGraphKey = altGraphKey;
		}
		
		#endregion
		
		#endregion
	}
}
