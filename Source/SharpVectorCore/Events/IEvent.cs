using System;

namespace SharpVectors.Dom.Events
{
	/// <summary>
	/// (DOM Level 2)
	/// The Event interface is used to provide contextual information about an
	/// event to the listener processing the event.
	/// </summary>
	/// <remarks>
	/// An object which implements the <c>IEvent</c> interface is passed as
	/// the parameter to an <see cref="EventListener">EventListener</see>.
	/// More specific context information is passed to event listeners by
	/// deriving additional interfaces from <see cref="IEvent">IEvent</see>
	/// which contain information directly relating to the type of event
	/// they represent. These derived interfaces are also implemented by
	/// the object passed to the event listener.
	/// </remarks>
	public interface IEvent
	{
		#region Properties
		
		#region DOM Level 2
		
		/// <summary>
		/// (DOM Level 2)
		/// The name must be an
		/// <see href="http://www.w3.org/TR/1999/REC-xml-names-19990114/#NT-NCName">NCName</see>
		/// as defined in
		/// [<see href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/references.html#Namespaces">XML Namespaces</see>]
		/// and is case-sensitive.
		/// </summary>
		/// <remarks>
		/// The character ":" (colon) should not be used in this attribute.
		/// If the attribute <see cref="IEvent.NamespaceUri">IEvent.NamespaceUri</see>
		/// is different from <c>null</c>, this attribute represents a
		/// <see href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/glossary.html#dt-localname">local name</see>.
		/// </remarks>
		string Type
		{
			get;
		}
		
		/// <summary>
		/// (DOM Level 2)
		/// Used to indicate the
		/// <see href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/glossary.html#dt-event-target">event target</see>.
		/// </summary>
		/// <remarks>
		/// This attribute contains the
		/// <see href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/glossary.html#dt-target-node">target node</see>
		/// when used with the
		/// <see href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/events.html#Events-flow">DOM event flow</see>.
		/// </remarks>
		IEventTarget Target
		{
			get;
		}
		
		/// <summary>
		/// (DOM Level 2)
		/// Used to indicate the <see cref="IEventTarget">IEventTarget</see>
		/// whose <see cref="EventListener">EventListener</see>s are
		/// currently being processed.
		/// </summary>
		/// <remarks>
		/// This is particularly useful during the capture and bubbling
		/// phases. This attribute could contain the
		/// <see href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/glossary.html#dt-target-node">target node</see>
		/// or a target ancestor when used with the
		/// <see href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/events.html#Events-flow">DOM event flow</see>.
		/// </remarks>
		IEventTarget CurrentTarget
		{
			get;
		}
		
		/// <summary>
		/// (DOM Level 2)
		/// Used to indicate which phase of event flow is currently being
		/// accomplished.
		/// </summary>
		EventPhase EventPhase
		{
			get;
		}
		
		/// <summary>
		/// (DOM Level 2)
		/// Used to indicate whether or not an event is a bubbling event.
		/// </summary>
		/// <remarks>
		/// If the event can bubble the value is <c>true</c>, otherwise the
		/// value is <c>false</c>.
		/// </remarks>
		bool Bubbles
		{
			get;
		}
		
		/// <summary>
		/// (DOM Level 2)
		/// Used to indicate whether or not an event can have its default
		/// action prevented.
		/// </summary>
		/// <remarks>
		/// If the default action can be prevented the value is <c>true</c>,
		/// otherwise the value is <c>false</c>.
		/// </remarks>
		/// <seealso href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/events.html#Events-flow-cancelation">Default actions and cancelable events</seealso>
		bool Cancelable
		{
			get;
		}
		
		/// <summary>
		/// (DOM Level 2)
		/// Used to specify the time at which the event was created.
		/// </summary>
		/// <remarks>
		/// Due to the fact that some systems may not provide this information
		/// the value of <see cref="TimeStamp">TimeStamp</see> may be not
		/// available for all events. When not available, a value of <c>0</c>
		/// will be returned. Examples of epoch time are the time of the
		/// system start or 0:0:0 UTC 1st January 1970.
		/// </remarks>
		DateTime TimeStamp
		{
			get;
		}
		
		#endregion
		
		#region DOM Level 3 Experimental
		
		/// <summary>
		/// (DOM Level 3 Experimental)
		/// The
		/// <see href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/glossary.html#dt-namespaceURI">namespace URI</see>
		/// associated with this event at creation time, or <c>null</c> if it
		/// is unspecified.
		/// </summary>
		/// <remarks>
		/// For events initialized with a DOM Level 2 Events method, such as
		/// <see cref="InitEvent">InitEvent</see>, this is always <c>null</c>.
		/// </remarks>
		string NamespaceUri
		{
			get;
		}
		
		/// <summary>
		/// (DOM Level 3 Experimental)
		/// This method will always return <c>false</c>, unless the event
		/// implements the <see cref="ICustomEvent">ICustomEvent</see>
		/// interface.
		/// </summary>
		/// <value>
		/// <c>true</c> if the event implements the
		/// <see cref="ICustomEvent">ICustomEvent</see> interface.
		/// <c>false</c> otherwise.
		/// </value>
		bool IsCustom
		{
			get;
		}
		
		/// <summary>
		/// (DOM Level 3 Experimental)
		/// This method will return true if the method
		/// <see cref="PreventDefault">PreventDefault</see> has been called
		/// for this event, <c>false</c> otherwise.
		/// </summary>
		/// <value>
		/// <c>true</c> if <see cref="PreventDefault">PreventDefault</see>
		/// has been called for this event.
		/// </value>
		bool IsDefaultPrevented
		{
			get;
		}
		
		#endregion
		
		#endregion
		
		#region Methods
		
		#region DOM Level 2
		
		/// <summary>
		/// (DOM Level 2)
		/// Prevent event listeners of the same group to be triggered.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method is used to prevent event listeners of the same group
		/// to be triggered but its effect is differed until all event
		/// listeners attached on the currentTarget have been triggered.
		/// Once it has been called, further calls to that method
		/// have no additional effect. 
		/// </para>
		/// <para>
		/// Note: This method does not prevent the default action
		/// from being invoked; use preventDefault for that effect.
		/// </para>
		/// </remarks>
		/// <seealso href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/events.html#Events-propagation-and-groups">
		/// Event propagation and event groups
		/// </seealso>
		void StopPropagation();
		
		/// <summary>
		/// (DOM Level 2)
		/// Signify that the event is to be canceled.
		/// </summary>
		/// <remarks>
		/// <para>
		/// If an event is cancelable, the
		/// <see cref="PreventDefault">PreventDefault</see> method is used to
		/// signify that the event is to be canceled, meaning any default
		/// action normally taken by the implementation as a result of the
		/// event will not occur and thus independently of event groups.
		/// Calling this method for a non-cancelable event has no effect.
		/// </para>
		/// <para>
		/// Note: This method does not stop the event propagation; use
		/// <see cref="StopPropagation">StopPropagation</see> or
		/// <see cref="StopImmediatePropagation">StopImmediatePropagation</see>
		/// for that effect.
		/// </para>
		/// </remarks>
		/// <seealso href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/events.html#Events-flow-cancelation">Default actions and cancelable events</seealso>
		void PreventDefault();
		
		/// <summary>
		/// (DOM Level 2)
		/// The <see cref="InitEvent">InitEvent</see> method is used to initialize
		/// the value of an <see cref="IEvent">IEvent</see> created through the
		/// <see cref="IDocumentEvent.CreateEvent">IDocumentEvent.CreateEvent</see>
		/// method.
		/// </summary>
		/// <remarks>
		/// This method may only be called before the
		/// <see cref="IEvent">IEvent</see> has been dispatched via the
		/// <see cref="IEventTarget.DispatchEvent">IEventTarget.DispatchEvent</see>
		/// method. If the method is called several times before invoking
		/// <see cref="IEventTarget.DispatchEvent">IEventTarget.DispatchEvent</see>,
		/// only the final invocation takes precedence. If called from a
		/// subclass of <see cref="IEvent">Event</see> interface only the
		/// values specified in this method are modified, all other
		/// attributes are left unchanged. This method sets the
		/// <see cref="IEvent.Type">IEvent.Type</see> attribute to
		/// <c>eventTypeArg</c>, and
		/// <see cref="IEvent.LocalName">IEvent.LocalName</see> and
		/// <see cref="IEvent.NamespaceUri">IEvent.NamespaceUri</see>
		/// to <c>null</c>. To initialize an event with a local name and
		/// namespace URI, use the
		/// <see cref="InitEventNs">InitEventNs</see> method.
		/// </remarks>
		/// <param name="eventTypeArg">
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
		void InitEvent(
			string eventTypeArg,
			bool canBubbleArg,
			bool cancelableArg);
		
		#endregion
		
		#region DOM Level 3 Experimental
		
		/// <summary>
		/// (DOM Level 3 Experimental)
		/// The <see>InitEventNs</see> method is used to initialize the value
		/// of an <see cref="IEvent">IEvent</see> created through the
		/// <see cref="IDocumentEvent">IDocumentEvent</see> interface.
		/// </summary>
		/// <remarks>
		/// This method may only be called before the
		/// <see cref="IEvent">IEvent</see> has been dispatched via the
		/// <see cref="IEventTarget.DispatchEvent">IEventTarget.DispatchEvent</see>
		/// method, though it may be called multiple times the event has been
		/// dispatched. If called multiple times the final invocation takes
		/// precedence. If a call to <see cref="InitEvent">InitEventNs</see>
		/// is made after one of the <see cref="IEvent">IEvent</see> derived
		/// interfaces' init methods has been called, only the values
		/// specified in the <see cref="InitEvent">InitEventNs</see> method
		/// are modified, all other attributes are left unchanged.
		/// This method sets the <see cref="IEvent.Type">IEvent.Type</see>
		/// attribute to <c>eventTypeArg</c>, and
		/// <see cref="IEvent.NamespaceUri">IEvent.NamespaceUri</see> to
		/// <c>namespaceUriArg</c>.
		/// </remarks>
		/// <param name="namespaceUriArg">
		/// Specifies the
		/// <see href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/glossary.html#dt-namespaceURI">namespace URI</see>
		/// associated with this event, or <c>null</c> if no namespace.
		/// </param>
		/// <param name="eventTypeArg">
		/// Specifies the
		/// <see href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/glossary.html#dt-localname">local name</see>
		/// of the event type (see also the description of
		/// <see cref="IEvent.Type">IEvent.Type</see>).
		/// </param>
		/// <param name="canBubbleArg">
		/// Specifies whether or not the event can bubble.
		/// </param>
		/// <param name="cancelableArg">
		/// Specifies whether or not the event's default action can be prevented.
		/// </param>
		void InitEventNs(
			string namespaceUriArg,
			string eventTypeArg,
			bool canBubbleArg,
			bool cancelableArg);
		
		/// <summary>
		/// (DOM Level 3 Experimental)
		/// Immediately prevent event listeners of the same group
		/// to be triggered.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method is used to prevent event listeners of the same group
		/// to be triggered and, unlike
		/// <see cref="StopPropagation">StopPropagation</see> its effect is
		/// immediate. Once it has been called, further calls to that method
		/// have no additional effect.
		/// </para>
		/// <para>
		/// Note: This method does not prevent the default action from being
		/// invoked; use <see cref="PreventDefault">PreventDefault</see> for
		/// that effect.
		/// </para>
		/// </remarks>
		/// <seealso href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/events.html#Events-propagation-and-groups">Event propagation and event groups</seealso>
		void StopImmediatePropagation();
		
		#endregion
		
		#endregion
	}
}
