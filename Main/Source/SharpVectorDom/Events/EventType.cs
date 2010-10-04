using System;

namespace SharpVectors.Dom.Events
{
	/// <summary>
	/// Summary description for MutationEventType.
	/// </summary>
	public sealed class EventType
	{
		public static readonly EventType DomActivate =
			new EventType(EventClasses.XmlEvents2001, "DOMActivate");
		public static readonly EventType DomFocusIn =
			new EventType(EventClasses.XmlEvents2001, "DOMFocusIn");
		public static readonly EventType DomFocusOut =
			new EventType(EventClasses.XmlEvents2001, "DOMFocusOut");
		public static readonly EventType TextInput =
			new EventType(EventClasses.XmlEvents2001, "textInput");
		public static readonly EventType Click =
			new EventType(EventClasses.XmlEvents2001, "click");
		public static readonly EventType MouseDown =
			new EventType(EventClasses.XmlEvents2001, "mousedown");
		public static readonly EventType MouseUp =
			new EventType(EventClasses.XmlEvents2001, "mouseup");
		public static readonly EventType MouseOver =
			new EventType(EventClasses.XmlEvents2001, "mouseover");
		public static readonly EventType MouseMove =
			new EventType(EventClasses.XmlEvents2001, "mousemove");
		public static readonly EventType MouseOut =
			new EventType(EventClasses.XmlEvents2001, "mouseout");
		public static readonly EventType KeyDown =
			new EventType(EventClasses.XmlEvents2001, "keydown");
		public static readonly EventType KeyUp =
			new EventType(EventClasses.XmlEvents2001, "keyup");
		public static readonly EventType DomSubtreeModified =
			new EventType(EventClasses.XmlEvents2001, "DOMSubtreeModified");
		public static readonly EventType DomNodeInserted =
			new EventType(EventClasses.XmlEvents2001, "DOMNodeInserted");
		public static readonly EventType DomNodeRemoved =
			new EventType(EventClasses.XmlEvents2001, "DOMNodeRemoved");
		public static readonly EventType DomNodeRemovedFromDocument =
			new EventType(EventClasses.XmlEvents2001, "DOMNodeRemovedFromDocument");
		public static readonly EventType DomNodeInsertedIntoDocument =
			new EventType(EventClasses.XmlEvents2001, "DOMNodeInsertedIntoDocument");
		public static readonly EventType DomAttrModified =
			new EventType(EventClasses.XmlEvents2001, "DOMAttrModified");
		public static readonly EventType DomCharacterDataModified =
			new EventType(EventClasses.XmlEvents2001, "DOMCharacterDataModified");
		public static readonly EventType DomElementNameChanged =
			new EventType(EventClasses.XmlEvents2001, "DOMElementNameChanged");
		public static readonly EventType DomAttributeNameChanged =
			new EventType(EventClasses.XmlEvents2001, "DOMAttributeNameChanged");
		public static readonly EventType Load =
			new EventType(EventClasses.XmlEvents2001, "load");
		public static readonly EventType Unload =
			new EventType(EventClasses.XmlEvents2001, "unload");
		public static readonly EventType Abort =
			new EventType(EventClasses.XmlEvents2001, "abort");
		public static readonly EventType Error =
			new EventType(EventClasses.XmlEvents2001, "error");
		public static readonly EventType Select =
			new EventType(EventClasses.XmlEvents2001, "select");
		public static readonly EventType Change =
			new EventType(EventClasses.XmlEvents2001, "change");
		public static readonly EventType Submit =
			new EventType(EventClasses.XmlEvents2001, "submit");
		public static readonly EventType Reset =
			new EventType(EventClasses.XmlEvents2001, "reset");
		public static readonly EventType Resize =
			new EventType(EventClasses.XmlEvents2001, "resize");
		public static readonly EventType Scroll =
			new EventType(EventClasses.XmlEvents2001, "scroll");
		
		#region Private Fields
		
		private string namespaceUri;
		private string eventType;
		
		#endregion
		
		#region Constructors
		
		private EventType(string namespaceUri, string eventType)
		{
			this.namespaceUri = namespaceUri;
			this.eventType    = eventType;
		}
		
		#endregion
		
		#region Public Properties
		
		public string Name
		{
			get
			{
				return eventType;
			}
		}
		
		public string NamespaceUri
		{
			get
			{
				return namespaceUri;
			}
		}
		
		#endregion
	}
}
