using System;
using SharpVectors.Dom.Views;

namespace SharpVectors.Dom.Events
{
	/// <summary>
	/// The <see cref="IUiEvent">IUiEvent</see> interface provides specific
	/// contextual information associated with User Interface events.
	/// </summary>
	/// <remarks>
	/// Note: To create an instance of the <see cref="IUiEvent">IUiEvent</see>
	/// interface, use the feature string <c>"UIEvent"</c> as the value of the
	/// input parameter used with the
	/// <see cref="IDocumentEvent.CreateEvent">IDocumentEvent.CreateEvent</see>
	/// method.
	/// </remarks>
	public interface IUiEvent
		: IEvent
	{
		/// <summary>
		/// The <see cref="View">View</see> attribute identifies the
		/// <see cref="IAbstractView">IAbstractView</see> from which the
		/// event was generated.
		/// </summary>
		IAbstractView View
		{
			get;
		}
		
		/// <summary>
		/// Specifies some detail information about the
		/// <see cref="IEvent">IEvent</see>, depending on the type of event.
		/// </summary>
		long Detail
		{
			get;
		}
		
		/// <summary>
		/// The <see cref="InitUiEvent">InitUiEvent</see> method is used to
		/// initialize the value of a <see cref="IUiEvent">IUiEvent</see>
		/// created using the
		/// <see cref="IDocumentEvent.CreateEvent">IDocumentEvent.CreateEvent</see>
		/// method.
		/// </summary>
		/// <remarks>
		/// This method may only be called before the
		/// <see cref="InitUiEvent">InitUiEvent</see> has been dispatched via
		/// the
		/// <see cref="IEventTarget.DispatchEvent">IEventTarget.DispatchEvent</see>
		/// method, though it may be called multiple times during that phase
		/// if necessary. If called multiple times, the final invocation takes
		/// precedence.
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
		/// prevented. This parameter overrides the intrinsic cancelable
		/// behavior of the event.
		/// </param>
		/// <param name="viewArg">
		/// Specifies the <see cref="IEvent">IEvent</see>'s
		/// <see cref="IAbstractView">IAbstractView</see>.
		/// </param>
		/// <param name="detailArg">
		/// Specifies the <see cref="IEvent">IEvent</see>'s detail.
		/// </param>
		void InitUiEvent(
			string typeArg,
			bool canBubbleArg,
			bool cancelableArg, 
			IAbstractView viewArg,
			long detailArg);
		
		/// <summary>
		/// The <see cref="InitUiEventNs">InitUiEventNs</see> method is used
		/// to initialize the value of a <see cref="IUiEvent">IUiEvent</see>
		/// created using the
		/// <see cref="IDocumentEvent.CreateEvent">IDocumentEvent.CreateEvent</see>
		/// method. This method may only be called before the
		/// <see cref="IUiEvent">IUiEvent</see> has been dispatched via the
		/// <see cref="IEventTarget.DispatchEvent">IEventTarget.DispatchEvent</see>
		/// method, though it may be called multiple times during that phase
		/// if necessary. If called multiple times, the final invocation
		/// takes precedence. 
		/// </summary>
		/// <param name="namespaceURI">
		/// Specifies the
		/// <see href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/glossary.html#dt-namespaceURI">namespace URI</see>
		/// associated with this event, or <c>null</c> if the application wish
		/// not to use namespaces.
		/// </param>
		/// <param name="typeArg">
		/// Specifies the event type (see also the description of the
		/// <c>type</c> attribute in the <see cref="IEvent">IEvent</see>
		/// interface).
		/// </param>
		/// <param name="canBubbleArg">
		/// Specifies whether or not the event can bubble.
		/// </param>
		/// <param name="cancelableArg">
		/// Specifies whether or not the event's default action can be prevented.
		/// </param>
		/// <param name="viewArg">
		/// Specifies the <see cref="IEvent">Event</see>'s
		/// <see cref="IAbstractView">IAbstractView</see>
		/// </param>
		/// <param name="detailArg">
		/// Specifies the <see cref="IEvent">IEvent</see>'s detail.
		/// </param>
		void InitUiEventNs(
			string namespaceURI,
			string typeArg,
			bool canBubbleArg,
			bool cancelableArg,
			IAbstractView viewArg,
			long detailArg);
	}
}
