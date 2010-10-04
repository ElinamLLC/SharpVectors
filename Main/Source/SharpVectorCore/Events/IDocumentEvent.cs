using System;

namespace SharpVectors.Dom.Events
{
	/// <summary>
	/// The <see cref="IDocumentEvent">IDocumentEvent</see> interface provides
	/// a mechanism by which the user can create an
	/// <see cref="IEvent">IEvent</see> object of a type supported by the
	/// implementation. It is expected that the
	/// <see cref="IDocumentEvent">IDocumentEvent</see> interface will be
	/// implemented on the same object which implements the
	/// <see cref="IDocument">IDocument</see> interface in an implementation
	/// which supports the <see cref="IEvent">IEvent</see> model.
	/// </summary>
	public interface IDocumentEvent
	{
		#region Methods
		
		#region DOM Level 2
		
		/// <summary>
		/// The <see cref="CreateEvent">CreateEvent</see> method is used in
		/// creating <see cref="IEvent">IEvent</see>s when it is either
		/// inconvenient or unnecessary for the user to create an
		/// <see cref="IEvent">IEvent</see> themselves.
		/// </summary>
		/// <remarks>
		/// <para>
		/// In cases where the implementation provided
		/// <see cref="IEvent">IEvent</see> is insufficient, users may supply
		/// their own <see cref="IEvent">IEvent</see> implementations for use
		/// with the
		/// <see cref="IEventTarget.DispatchEvent">IEventTarget.DispatchEvent</see>
		/// method. However, the DOM implementation needs access to the
		/// attributes
		/// <see cref="IEvent.CurrentTarget">IEvent.CurrentTarget</see> and
		/// <see cref="IEvent.EventPhase">IEvent.EventPhase</see> to propagate
		/// appropriately the event in the DOM tree. Therefore users'
		/// <see cref="IEvent">IEvent</see> implementations might need to
		/// support the <see cref="ICustomEvent">ICustomEvent</see> interface
		/// for that effect.
		/// </para>
		/// <para>
		/// Note: For backward compatibility reason, <c>"UIEvents"</c>,
		/// <c>"MouseEvents"</c>, <c>"MutationEvents"</c>, and
		/// <c>"HTMLEvents"</c> feature names are valid values for the
		/// parameter <c>eventType</c> and represent respectively the
		/// interfaces <c>"UIEvent"</c>, <c>"MouseEvent"</c>,
		/// <c>"MutationEvent"</c>, and <c>"Event"</c>.
		/// </para>
		/// </remarks>
		/// <param name="eventType">
		/// The <c>eventType</c> parameter specifies the name of the DOM
		/// Events interface to be supported by the created event object, e.g.
		/// <c>"Event"</c>, <c>"MouseEvent"</c>, <c>"MutationEvent"</c> ...
		/// If the <see cref="IEvent">IEvent</see> is to be dispatched via the
		/// <see cref="IEventTarget.DispatchEvent">IEventTarget.DispatchEvent</see>
		/// method the appropriate event init method must be called after
		/// creation in order to initialize the <see cref="IEvent">IEvent</see>'s
		/// values.  As an example, a user wishing to synthesize some kind of
		/// <see cref="IUiEvent">IUiEvent</see> would call
		/// <see cref="IDocumentEvent.CreateEvent">IDocumentEvent.CreateEvent</see>
		/// with the parameter <c>"UIEvent"</c>. The
		/// <see cref="IUiEvent.InitUiEventNs">IUiEvent.InitUiEventNs</see>
		/// method could then be called on the newly created
		/// <see cref="IUiEvent">IUiEvent</see> object to set the specific
		/// type of user interface event to be dispatched,
		/// {<c>"http://www.w3.org/2001/xml-events"</c>,
		/// <c>"DOMActivate"</c>} for example, and set its context
		/// information, e.g.
		/// <see cref="IUiEvent.Detail">IUiEvent.Detail</see> in this example.
		/// </param>
		/// <returns>
		/// The newly created event object.
		/// </returns>
		/// <exception cref="DomException">
		/// NOT_SUPPORTED_ERR: Raised if the implementation does not support
		/// the <see cref="IEvent">Event</see> interface requested.
		/// </exception>
		IEvent CreateEvent(
			string eventType);
		
		#endregion
		
		#region DOM Level 3 Experimental
		
		/// <summary>
		/// Test if the implementation can generate events of a specified type.
		/// </summary>
		/// <param name="namespaceUri">
		/// Specifies the
		/// <see cref="IEvent.NamespaceUri">IEvent.NamespaceUri</see> of the
		/// event.
		/// </param>
		/// <param name="type">
		/// Specifies the <see cref="IEvent.Type">IEvent.Type</see> of the
		/// event.
		/// </param>
		/// <returns>
		/// <c>true</c> if the implementation can generate and dispatch this
		/// event type, <c>false</c> otherwise.
		/// </returns>
		bool CanDispatch(
			string namespaceUri,
			string type);
		
		#endregion
		
		#endregion
	}
}
