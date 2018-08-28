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
		
		private bool _ctrlKey;
		private bool _shiftKey;
		private bool _altKey;
		private bool _metaKey;
		private bool _altGraphKey;
		private string _keyIdentifier;
		private KeyLocationCode _keyLocation;
		
		#endregion
		
		#region Constructors
		
		public KeyboardEvent()
		{
		}
		
		public KeyboardEvent(string eventType, bool bubbles, bool cancelable, IAbstractView view,
			string keyIdentifier, KeyLocationCode keyLocation, bool ctrlKey, bool shiftKey,
			bool altKey, bool metaKey, bool altGraphKey)
		{
			InitKeyboardEvent(eventType, bubbles, cancelable, view, keyIdentifier, keyLocation,
				ctrlKey, shiftKey, altKey, metaKey, altGraphKey);
		}
		
		public KeyboardEvent(string namespaceUri, string eventType, bool bubbles, bool cancelable,
			IAbstractView view, string keyIdentifier, KeyLocationCode keyLocation, bool ctrlKey,
			bool shiftKey, bool altKey, bool metaKey, bool altGraphKey)
		{
			InitKeyboardEventNs(namespaceUri, eventType, bubbles, cancelable, view,
				keyIdentifier, keyLocation, ctrlKey, shiftKey, altKey, metaKey, altGraphKey);
		}
		
		#endregion
		
		#region IKeyboardEvent interface
		
		public string KeyIdentifier
		{
			get
			{
				return _keyIdentifier;
			}
		}
		
		public KeyLocationCode KeyLocation
		{
			get
			{
				return _keyLocation;
			}
		}
		
		public bool CtrlKey
		{
			get
			{
				return _ctrlKey;
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
		
		public bool AltGraphKey
		{
			get
			{
				return _altGraphKey;
			}
		}
		
		public void InitKeyboardEvent(string eventType, bool bubbles, bool cancelable, 
            IAbstractView view, string keyIdentifier, KeyLocationCode keyLocation, 
            bool ctrlKey, bool shiftKey, bool altKey, bool metaKey, bool altGraphKey)
		{
			InitUiEvent(eventType, bubbles, cancelable, view, 0);
			
			_keyIdentifier = keyIdentifier;
			_keyLocation   = keyLocation;
			_ctrlKey       = ctrlKey;
			_shiftKey      = shiftKey;
			_altKey        = altKey;
			_metaKey       = metaKey;
			_altGraphKey   = altGraphKey;
		}
		
		public void InitKeyboardEventNs(string namespaceUri, string eventType, bool bubbles,
			bool cancelable, IAbstractView view, string keyIdentifier, KeyLocationCode keyLocation,
			bool ctrlKey, bool shiftKey, bool altKey, bool metaKey, bool altGraphKey)
		{
			InitUiEventNs(namespaceUri, eventType, bubbles, cancelable, view, 0);
			
			_keyIdentifier = keyIdentifier;
			_keyLocation   = keyLocation;
			_ctrlKey       = ctrlKey;
			_shiftKey      = shiftKey;
			_altKey        = altKey;
			_metaKey       = metaKey;
			_altGraphKey   = altGraphKey;
		}
		
		#endregion
	}
}
