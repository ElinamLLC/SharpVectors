using System;

using SharpVectors.Dom.Views;

namespace SharpVectors.Dom.Events
{
	/// <summary>
	/// The <see cref="IKeyboardEvent">IKeyboardEvent</see> interface provides
	/// specific contextual information associated with keyboard devices.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Each keyboard event reference a key using an identifier.
	/// </para>
	/// <para>
	/// Each modifier attribute
	/// (<see cref="CtrlKey">CtrlKey</see>,
	/// <see cref="ShiftKey">ShiftKey</see>,
	/// <see cref="AltKey">AltKey</see>,
	/// <see cref="MetaKey">MetaKey</see>,
	/// and <see cref="AltGraphKey">AltGraphKey</see>) is activated when the
	/// key modifier is being pressed down or maintained pressed, i.e. the
	/// modifier attribute is not in use when the key modifier is being
	/// released.
	/// </para>
	/// <para>
	/// Note: To create an instance of the
	/// <see cref="IKeyboardEvent">IKeyboardEvent</see> interface, use the
	/// feature string <c>"KeyboardEvent"</c> as the value of the input
	/// parameter used with the
	/// <see cref="IDocumentEvent.CreateEvent">IDocumentEvent.CreateEvent</see>
	/// method.
	/// </para>
	/// </remarks>
	public interface IKeyboardEvent
		: IUiEvent
	{
		/// <summary>
		/// Holds the identifier of the key.
		/// </summary>
		/// <remarks>
		/// <para>
		/// For a list of possible values, refer to
		/// <see href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/keyset.html#KeySet">Key identifiers for keyboard events</see>.
		/// </para>
		/// <para>
		/// Note: Implementations that are unable to identify a key must use the key identifier "Unidentified".
		/// </para>
		/// </remarks>
		string KeyIdentifier
		{
			get;
		}
		
		/// <summary>
		/// Contains an indication of the location of they key on the device.
		/// </summary>
		KeyLocationCode KeyLocation
		{
			get;
		}
		
		/// <summary>
		/// <c>true</c> if the control (Ctrl) key modifier is activated.
		/// </summary>
		bool CtrlKey
		{
			get;
		}
		
		/// <summary>
		/// <c>true</c> if the shift (Shift) key modifier is activated.
		/// </summary>
		bool ShiftKey
		{
			get;
		}
		
		/// <summary>
		/// <c>true</c> if the alt (alternative) key modifier is activated.
		/// </summary>
		bool AltKey
		{
			get;
		}
		
		/// <summary>
		/// <c>true</c> if the meta (Meta) key modifier is activated.
		/// </summary>
		/// <remarks>
		/// Note: The Command key modifier on Macintosh system must be
		/// represented using this key modifier.
		/// </remarks>
		bool MetaKey
		{
			get;
		}
		
		/// <summary>
		/// true if the alt-graph (Alt Gr) key modifier is activated. 
		/// </summary>
		/// <remarks>
		/// Note: Some operating systems simulate the alt-graph key modifier
		/// with the combinaison of alt and ctrl key modifiers.
		/// Implementations are encouraged to use this modifier instead.
		/// </remarks>
		bool AltGraphKey
		{
			get;
		}
		
		/// <summary>
		/// The <see cref="InitKeyboardEvent">InitKeyboardEvent</see> method
		/// is used to initialize the value of a
		/// <see cref="IKeyboardEvent">IKeyboardEvent</see> created using the
		/// <see cref="IDocumentEvent.CreateEvent">IDocumentEvent.CreateEvent</see>
		/// method.
		/// </summary>
		/// <remarks>
		/// This method may only be called before the
		/// <see cref="IKeyboardEvent">IKeyboardEvent</see> has been
		/// dispatched via the
		/// <see cref="IEventTarget.DispatchEvent">IEventTarget.DispatchEvent</see>
		/// method, though it may be called multiple times before being
		/// dispatched if necessary. If called multiple times, the final
		/// invocation takes precedence. This method has no effect if called
		/// after the event has been dispatched.
		/// </remarks>
		/// <param name="typeArg">
		/// Specifies the event type.
		/// </param>
		/// <param name="canBubbleArg">
		/// Specifies whether or not the event can bubble. This parameter
		/// overrides the intrinsic bubbling behavior of the event.
		/// </param>
		/// <param name="cancelableArg">
		/// Specifies whether or not the event's default action can be
		/// prevent. This parameter overrides the intrinsic cancelable
		/// behavior of the event.
		/// </param>
		/// <param name="viewArg">
		/// Specifies the <see cref="IKeyboardEvent">IKeyboardEvent</see>'s
		/// <see cref="IAbstractView">IAbstractView</see>.
		/// </param>
		/// <param name="keyIdentifierArg">
		/// Specifies the <see cref="IKeyboardEvent">IKeyboardEvent</see>'s
		/// <see cref="KeyIdentifier">KeyIdentifier</see> attribute.
		/// </param>
		/// <param name="keyLocationArg">
		/// Specifies the <see cref="IKeyboardEvent">IKeyboardEvent</see>'s
		/// <see cref="KeyLocation">KeyLocation</see> attribute.
		/// </param>
		/// <param name="ctrlKeyArg">
		/// Specifies the <see cref="IKeyboardEvent">IKeyboardEvent</see>'s
		/// <see cref="CtrlKey">CtrlKey</see> attribute.
		/// </param>
		/// <param name="shiftKeyArg">
		/// Specifies the <see cref="IKeyboardEvent">IKeyboardEvent</see>'s
		/// <see cref="ShiftKey">ShiftKey</see> attribute.
		/// </param>
		/// <param name="altKeyArg">
		/// Specifies the <see cref="IKeyboardEvent">IKeyboardEvent</see>'s
		/// <see cref="AltKey">AltKey</see> attribute.
		/// </param>
		/// <param name="metaKeyArg">
		/// Specifies the <see cref="IKeyboardEvent">IKeyboardEvent</see>'s
		/// <see cref="MetaKey">MetaKey</see> attribute.
		/// </param>
		/// <param name="altGraphKeyArg">
		/// Specifies the <see cref="IKeyboardEvent">IKeyboardEvent</see>'s
		/// <see cref="AltGraphKey">AltGraphKey</see> attribute.
		/// </param>
		void InitKeyboardEvent(
			string typeArg,
			bool canBubbleArg,
			bool cancelableArg,
			IAbstractView viewArg,
			string keyIdentifierArg,
			KeyLocationCode keyLocationArg,
			bool ctrlKeyArg,
			bool shiftKeyArg,
			bool altKeyArg,
			bool metaKeyArg,
			bool altGraphKeyArg);
		
		/// <summary>
		/// The <see cref="InitKeyboardEventNs">InitKeyboardEventNs</see>
		/// method is used to initialize the value of a
		/// <see cref="IKeyboardEvent">IKeyboardEvent</see> created using the
		/// <see cref="IDocumentEvent.CreateEvent">IDocumentEvent.CreateEvent</see>
		/// method.
		/// </summary>
		/// <remarks>
		/// This method may only be called before the
		/// <see cref="IKeyboardEvent">IKeyboardEvent</see> has been
		/// dispatched via the
		/// <see cref="IEventTarget.DispatchEvent">IEventTarget.DispatchEvent</see>
		/// method, though it may be called multiple times during that phase
		/// if necessary. If called multiple times, the final invocation
		/// takes precedence. This method has no effect if called after the
		/// event has been dispatched.
		/// </remarks>
		/// <param name="namespaceUri">
		/// Specifies the
		/// <see href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/glossary.html#dt-namespaceURI">namespace URI</see>
		/// associated with this event, or null if the applications wish to
		/// have no namespace.
		/// </param>
		/// <param name="type">
		/// Specifies the event type.
		/// </param>
		/// <param name="canBubbleArg">
		/// Specifies whether or not the event can bubble.
		/// </param>
		/// <param name="cancelableArg">
		/// Specifies whether or not the event's default action can be
		/// prevent.
		/// </param>
		/// <param name="viewArg">
		/// Specifies the <see cref="IKeyboardEvent">IKeyboardEvent</see>'s
		/// <see cref="IAbstractView">IAbstractView</see>.
		/// </param>
		/// <param name="keyIdentifierArg">
		/// Specifies the <see cref="IKeyboardEvent">IKeyboardEvent</see>'s
		/// <see cref="KeyIdentifier">KeyIdentifier</see> attribute.
		/// </param>
		/// <param name="keyLocationArg">
		/// Specifies the <see cref="IKeyboardEvent">IKeyboardEvent</see>'s
		/// <see cref="KeyLocation">KeyLocation</see> attribute.
		/// </param>
		/// <param name="ctrlKeyArg">
		/// Specifies the <see cref="IKeyboardEvent">IKeyboardEvent</see>'s
		/// <see cref="CtrlKey">CtrlKey</see> attribute.
		/// </param>
		/// <param name="shiftKeyArg">
		/// Specifies the <see cref="IKeyboardEvent">IKeyboardEvent</see>'s
		/// <see cref="ShiftKey">ShiftKey</see> attribute.
		/// </param>
		/// <param name="altKeyArg">
		/// Specifies the <see cref="IKeyboardEvent">IKeyboardEvent</see>'s
		/// <see cref="AltKey">AltKey</see> attribute.
		/// </param>
		/// <param name="metaKeyArg">
		/// Specifies the <see cref="IKeyboardEvent">IKeyboardEvent</see>'s
		/// <see cref="MetaKey">MetaKey</see> attribute.
		/// </param>
		/// <param name="altGraphKeyArg">
		/// Specifies the <see cref="IKeyboardEvent">IKeyboardEvent</see>'s
		/// <see cref="AltGraphKey">AltGraphKey</see> attribute.
		/// </param>
		void InitKeyboardEventNs(
			string namespaceUri,
			string type,
			bool canBubbleArg,
			bool cancelableArg,
			IAbstractView viewArg,
			string keyIdentifierArg,
			KeyLocationCode keyLocationArg,
			bool ctrlKeyArg,
			bool shiftKeyArg,
			bool altKeyArg,
			bool metaKeyArg,
			bool altGraphKeyArg);
	}
}
