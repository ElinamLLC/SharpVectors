using System;
using SharpVectors.Dom.Views;

namespace SharpVectors.Dom.Events
{
	/// <summary>
	/// The <see cref="IMouseEvent">IMouseEvent</see> interface provides
	/// specific contextual information associated with Mouse events.
	/// </summary>
	/// <remarks>
	/// <para>
	/// In the case of nested elements mouse events are always targeted at
	/// the most deeply nested element. Ancestors of the targeted element
	/// may use bubbling to obtain notification of mouse events which
	/// occur within its descendent elements.
	/// </para>
	/// <para>
	/// Note: To create an instance of the MouseEvent interface, use the
	/// feature string "MouseEvent" as the value of the input parameter
	/// used with the DocumentEvent.createEvent method.
	/// </para>
	/// </remarks>
	public interface IMouseEvent
		: IUiEvent
	{
		/// <summary>
		/// The horizontal coordinate at which the event occurred relative
		/// to the origin of the screen coordinate system.
		/// </summary>
		long ScreenX
		{
			get;
		}
		
		/// <summary>
		/// The vertical coordinate at which the event occurred relative to
		/// the origin of the screen coordinate system.
		/// </summary>
		long ScreenY
		{
			get;
		}
		
		/// <summary>
		/// The horizontal coordinate at which the event occurred relative
		/// to the DOM implementation's client area.
		/// </summary>
		long ClientX
		{
			get;
		}
		
		/// <summary>
		/// The vertical coordinate at which the event occurred relative to
		/// the DOM implementation's client area.
		/// </summary>
		long ClientY
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
		/// Note: The Command key modifier on Macintosh system must be represented using this key modifier.
		/// </remarks>
		bool MetaKey
		{
			get;
		}
		
		/// <summary>
		/// During mouse events caused by the depression or release of a mouse
		/// button, button is used to indicate which mouse button changed
		/// state.
		/// </summary>
		/// <remarks>
		/// <c>0</c> indicates the normal (in general on the left or
		/// the one button on Macintosh mice, used to activate a button or
		/// select text) button of the mouse. <c>2</c> indicates the contextual
		/// property (in general on the right, used to display a context menu)
		/// button of the mouse if present. <c>1</c> indicates the extra (in
		/// general in the middle and often combined with the mouse wheel)
		/// button. Some mice may provide or simulate more buttons and values
		/// higher than <c>2</c> could be used to represent such buttons.
		/// </remarks>
		ushort Button
		{
			get;
		}
		
		/// <summary>
		/// Used to identify a secondary EventTarget related to a UI event.
		/// </summary>
		/// <remarks>
		/// Currently this attribute is used with the mouseover event to
		/// indicate the <see cref="IEventTarget">IEventTarget</see> which
		/// the pointing device exited and with the mouseout event to
		/// indicate the <see cref="IEventTarget">IEventTarget</see> which
		/// the pointing device entered.
		/// </remarks>
		IEventTarget RelatedTarget
		{
			get;
		}
		
		/// <summary>
		/// <c>true</c> if the alt-graph (Alt Gr) key modifier is activated.
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
		/// The <see cref="InitMouseEvent">InitMouseEvent</see> method is used
		/// to initialize the value of a MouseEvent created using the
		/// <see cref="IDocumentEvent.CreateEvent">IDocumentEvent.CreateEvent</see>
		/// method.
		/// </summary>
		/// <remarks>
		/// This method may only be called before the
		/// <see cref="IMouseEvent">IMouseEvent</see> has been dispatched via
		/// the
		/// <see cref="IEventTarget.DispatchEvent">IEventTarget.DispatchEvent</see>
		/// method, though it may be called multiple times before being
		/// dispatched. If called multiple times, the final invocation
		/// takes precedence. 
		/// </remarks>
		/// <param name="typeArg">
		/// Specifies the event type.
		/// </param>
		/// <param name="canBubbleArg">
		/// Specifies whether or not the event can bubble.
		/// </param>
		/// <param name="cancelableArg">
		/// Specifies whether or not the event's default action can be
		/// prevented.
		/// </param>
		/// <param name="viewArg">
		/// Specifies the <see cref="IEvent">IEvent</see>'s view.
		/// </param>
		/// <param name="detailArg">
		/// Specifies the <see cref="IEvent">IEvent</see>'s mouse click count.
		/// </param>
		/// <param name="screenXArg">
		/// Specifies the <see cref="IEvent">IEvent</see>'s screen x
		/// coordinate.
		/// </param>
		/// <param name="screenYArg">
		/// Specifies the <see cref="IEvent">IEvent</see>'s screen y
		/// coordinate.
		/// </param>
		/// <param name="clientXArg">
		/// Specifies the <see cref="IEvent">IEvent</see>'s client x
		/// coordinate.
		/// </param>
		/// <param name="clientYArg">
		/// Specifies the <see cref="IEvent">IEvent</see>'s client y
		/// coordinate.
		/// </param>
		/// <param name="ctrlKeyArg">
		/// Specifies whether or not control key was depressed during
		/// the <see cref="IEvent">IEvent</see>.
		/// </param>
		/// <param name="altKeyArg">
		/// Specifies whether or not alt key was depressed during the
		/// <see cref="IEvent">IEvent</see>.
		/// </param>
		/// <param name="shiftKeyArg">
		/// Specifies whether or not shift key was depressed during the
		/// <see cref="IEvent">IEvent</see>.
		/// </param>
		/// <param name="metaKeyArg">
		/// Specifies whether or not meta key was depressed during the
		/// <see cref="IEvent">IEvent</see>.
		/// </param>
		/// <param name="buttonArg">
		/// Specifies the <see cref="IEvent">IEvent</see>'s mouse button.
		/// </param>
		/// <param name="relatedTargetArg">
		/// Specifies the <see cref="IEvent">IEvent</see>'s related
		/// <see cref="IEventTarget">IEventTarget</see>.
		/// </param>
		void InitMouseEvent(
			string typeArg,
			bool canBubbleArg,
			bool cancelableArg,
			IAbstractView viewArg,
			long detailArg,
			long screenXArg,
			long screenYArg,
			long clientXArg,
			long clientYArg,
			bool ctrlKeyArg,
			bool altKeyArg,
			bool shiftKeyArg,
			bool metaKeyArg,
			ushort buttonArg,
			IEventTarget relatedTargetArg);
		
		/// <summary>
		/// The <see cref="InitMouseEventNs">InitMouseEventNs</see> method
		/// is used to initialize the value of a
		/// <see cref="IMouseEvent">IMouseEvent</see> created using the
		/// <see cref="IDocumentEvent.CreateEvent">IDocumentEvent.CreateEvent</see>
		/// interface.
		/// </summary>
		/// <remarks>
		/// This method may only be called before the
		/// <see cref="IMouseEvent">IMouseEvent</see> has been dispatched via
		/// the
		/// <see cref="IEventTarget.DispatchEvent">IEventTarget.DispatchEvent</see>
		/// method, though it may be called multiple times during that phase
		/// if necessary. If called multiple times, the final invocation takes
		/// precedence.
		/// </remarks>
		/// <param name="namespaceUri">
		/// Specifies the
		/// <see href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/glossary.html#dt-namespaceURI">namespace URI</see>
		/// associated with this event, or null if the application wish to
		/// have no namespace.
		/// </param>
		/// <param name="typeArg">
		/// Specifies the event type.
		/// </param>
		/// <param name="canBubbleArg">
		/// Specifies whether or not the event can bubble.
		/// </param>
		/// <param name="cancelableArg">
		/// Specifies whether or not the event's default action can be
		/// prevented.
		/// </param>
		/// <param name="viewArg">
		/// Specifies the <see cref="IEvent">IEvent</see>'s AbstractView.
		/// </param>
		/// <param name="detailArg">
		/// Specifies the <see cref="IEvent">IEvent</see>'s mouse click count.
		/// </param>
		/// <param name="screenXArg">
		/// Specifies the <see cref="IEvent">IEvent</see>'s screen x
		/// coordinate.
		/// </param>
		/// <param name="screenYArg">
		/// Specifies the <see cref="IEvent">IEvent</see>'s screen y
		/// coordinate.
		/// </param>
		/// <param name="clientXArg">
		/// Specifies the <see cref="IEvent">IEvent</see>'s client x
		/// coordinate.
		/// </param>
		/// <param name="clientYArg">
		/// Specifies the <see cref="IEvent">IEvent</see>'s client y
		/// coordinate.
		/// </param>
		/// <param name="ctrlKeyArg">
		/// Specifies whether or not control key was depressed during the
		/// <see cref="IEvent">IEvent</see>.
		/// </param>
		/// <param name="altKeyArg">
		/// Specifies whether or not alt key was depressed during the
		/// <see cref="IEvent">IEvent</see>.
		/// </param>
		/// <param name="shiftKeyArg">
		/// Specifies whether or not shift key was depressed during the
		/// <see cref="IEvent">IEvent</see>.
		/// </param>
		/// <param name="metaKeyArg">
		/// Specifies whether or not meta key was depressed during the Event.
		/// </param>
		/// <param name="buttonArg">
		/// Specifies the <see cref="IEvent">IEvent</see>'s mouse button.
		/// </param>
		/// <param name="relatedTargetArg">
		/// Specifies the <see cref="IEvent">IEvent</see>'s related
		/// <see cref="IEventTarget">IEventTarget</see>.
		/// </param>
		/// <param name="altGraphKeyArg">
		/// Specifies whether or not alt graph key was depressed during the
		/// <see cref="IEvent">IEvent</see>.
		/// </param>
		void InitMouseEventNs(
			string namespaceUri,
			string typeArg,
			bool canBubbleArg,
			bool cancelableArg,
			IAbstractView viewArg,
			long detailArg,
			long screenXArg,
			long screenYArg,
			long clientXArg,
			long clientYArg,
			bool ctrlKeyArg,
			bool altKeyArg,
			bool shiftKeyArg,
			bool metaKeyArg,
			ushort buttonArg,
			IEventTarget relatedTargetArg,
			bool altGraphKeyArg);
	}
}