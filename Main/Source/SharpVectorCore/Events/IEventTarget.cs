using System;

namespace SharpVectors.Dom.Events
{
	/// <summary>
	/// The <see cref="IEventTarget">IEventTarget</see> interface is
	/// implemented by all the objects which could be
	/// <see href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/glossary.html#dt-event-target">event targets</see>
	/// in an implementation which supports the
	/// <see href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/events.html#Events-flows">Event flows</see>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The interface allows registration, removal or query of event
	/// listeners, and dispatch of events to an event target.
	/// </para>
	/// <para>
	/// When used with
	/// <see href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/events.html#Events-flow">DOM event flow</see>,
	/// this interface is implemented by all
	/// <see href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/glossary.html#dt-target-node">target nodes</see>
	/// and target ancestors, i.e. all DOM <see cref="INode">INode</see>s of
	/// the tree support this interface when the implementation conforms to
	/// DOM Level 3 Events and, therefore, this interface can be obtained by
	/// using binding-specific casting methods on an instance of the
	/// <see cref="INode">Node</see> interface.
	/// </para>
	/// <para>
	/// Invoking multiple times
	/// <see cref="AddEventListener">AddEventListener</see> or
	/// <see cref="AddEventListenerNs">AddEventListenerNs</see> on the same
	/// <see cref="IEventTarget">IEventTarget</see> with the same parameters
	/// (<c>namespaceUri</c>, <c>type</c>, <c>listener</c>, and
	/// <c>useCapture</c>) is considered to be a no-op and thus independently
	/// of the event group. They do not cause the
	/// <see cref="EventListener">EventListener</see> to be called more
	/// than once and do not cause a change in the triggering order. In order
	/// to guarantee that an event listener will be added to the event target
	/// for the specified event group, one needs to invoke
	/// <see cref="RemoveEventListener">RemoveEventListener</see> or
	/// <see cref="RemoveEventListenerNs">RemoveEventListenerNs</see> first.
	/// </para>
	/// </remarks>
	public interface IEventTarget
	{
		#region Methods
		
		#region DOM Level 2
		
		/// <summary>
		/// This method allows the registration of an event listener in the
		/// default group and, depending on the <c>useCapture</c> parameter,
		/// on the capture phase of the DOM event flow or its target and
		/// bubbling phases. <see cref=" http://www.w3.org/TR/SVG/interact.html#SVGEvents"/>
		/// </summary>
		/// <param name="type">
		/// Specifies the <see cref="IEvent.Type">IEvent.Type</see> associated
		/// with the event for which the user is registering. 
		/// </param>
		/// <param name="listener">
		/// The listener parameter takes an object implemented by the user
		/// which implements the
		/// <see cref="EventListener">EventListener</see> interface and
		/// contains the method to be called when the event occurs.
		/// </param>
		/// <param name="useCapture">
		/// If <c>true</c>, <c>useCapture</c> indicates that the user wishes
		/// to add the event listener for the
		/// <see href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/glossary.html#dt-capture-phase">capture phase</see>
		/// only, i.e. this event listener will not be triggered during the
		/// <see href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/glossary.html#dt-target-phase">target</see>
		/// and
		/// <see href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/glossary.html#dt-bubbling-phase">bubbling phases</see>.
		/// If <c>false</c>, the event listener will only be triggered during the target and bubbling phases.
		/// </param>
		void AddEventListener(
			string type,
			EventListener listener,
			bool useCapture);
		
		/// <summary>
		/// This method allows the removal of event listeners from the default
		/// group.
		/// </summary>
		/// <remarks>
		/// Calling <see cref="RemoveEventListener">RemoveEventListener</see>
		/// with arguments which do not identify any currently registered
		/// <see cref="EventListener">EventListener</see> on the
		/// <see cref="IEventTarget">IEventTarget</see> has no effect. 
		/// </remarks>
		/// <param name="type">
		/// Specifies the <see cref="IEvent.Type">IEvent.Type</see> for which
		/// the user registered the event listener.
		/// </param>
		/// <param name="listener">
		/// The <see cref="EventListener">EventListener</see> to be removed.
		/// </param>
		/// <param name="useCapture">
		/// Specifies whether the
		/// <see cref="EventListener">EventListener</see> being removed was
		/// registered for the capture phase or not. If a listener was
		/// registered twice, once for the capture phase and once for the
		/// target and bubbling phases, each must be removed separately.
		/// Removal of an event listener registered for the capture phase does
		/// not affect the same event listener registered for the target and
		/// bubbling phases, and vice versa.
		/// </param>
		void RemoveEventListener(
			string type,
			EventListener listener,
			bool useCapture);
		
		/// <summary>
		/// This method allows the dispatch of events into the
		/// implementation's event model.
		/// </summary>
		/// <remarks>
		/// The
		/// <see href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/glossary.html#dt-event-target">event target</see>
		/// of the event is the <see cref="IEventTarget">IEventTarget</see>
		/// object on which <see cref="DispatchEvent">DispatchEvent</see>
		/// is called.
		/// </remarks>
		/// <param name="evt">
		/// The event to be dispatched.
		/// </param>
		/// <returns>
		/// Indicates whether any of the listeners which handled the event
		/// called
		/// <see cref="IEvent.PreventDefault">IEvent.PreventDefault</see>.
		/// If <see cref="IEvent.PreventDefault">IEvent.PreventDefault</see>
		/// was called the returned value is <c>false</c>, else it is
		/// <c>true</c>.
		/// </returns>
		/// <exception cref="EventException">
		/// <para>
		/// UNSPECIFIED_EVENT_TYPE_ERR: Raised if the Event.type was not
		/// specified by initializing the event before dispatchEvent was
		/// called. Specification of the Event.type as null or an empty
		/// string will also trigger this exception.
		/// </para>
		/// <para>
		/// DISPATCH_REQUEST_ERR: Raised if the Event object is already being
		/// dispatched in the tree.
		/// </para>
		/// <para>
		/// NOT_SUPPORTED_ERR: Raised if the Event object has not been
		/// created using DocumentEvent.createEvent or does not support the
		/// interface CustomEvent.
		/// </para>
		/// </exception>
		bool DispatchEvent(
			IEvent evt);
		
		#endregion
		
		#region DOM Level 3 Experimental
		
		/// <summary>
		/// This method allows the registration of an event listener in a
		/// specified group or the default group and, depending on the
		/// <c>useCapture</c> parameter, on the capture phase of the DOM
		/// event flow or its target and bubbling phases.
		/// </summary>
		/// <param name="namespaceUri">
		/// Specifies the
		/// <see cref="IEvent.NamespaceUri">IEvent.NamespaceUri</see>
		/// associated with the event for which the user is registering.
		/// </param>
		/// <param name="type">
		/// Specifies the <see cref="IEvent.Type">IEvent.Type</see>
		/// associated with the event for which the user is registering.
		/// </param>
		/// <param name="listener">
		/// The <c>listener</c> parameter takes an object implemented by
		/// the user which implements the
		/// <see cref="EventListener">EventListener</see> interface and
		/// contains the method to be called when the event occurs.
		/// </param>
		/// <param name="useCapture">
		/// If <c>true</c>, <c>useCapture</c> indicates that the user wishes
		/// to add the event listener for the
		/// <see href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/glossary.html#dt-capture-phase">capture phase only</see>,
		/// i.e. this event listener will not be triggered during the
		/// <see href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/glossary.html#dt-target-phase">target</see>
		/// and
		/// <see href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/glossary.html#dt-bubbling-phase">bubbling phases</see>.
		/// If <c>false</c>, the event listener will only be triggered
		/// during the target and bubbling phases.
		/// </param>
		/// <param name="evtGroup">
		/// The object that represents the event group to associate with the
		/// <see cref="EventListener">EventListener</see>. Use <c>null</c> to
		/// attach the event listener to the default group.
		/// </param>
		/// <seealso href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/events.html#Events-propagation-and-groups">
		/// Event propagation and event groups
		/// </seealso>
		void AddEventListenerNs(
			string namespaceUri,
			string type,
			EventListener listener,
			bool useCapture,
			object evtGroup);
		
		/// <summary>
		/// This method allows the removal of event listeners from a specified
		/// group or the default group.
		/// </summary>
		/// <remarks>
		/// Calling
		/// <see cref="RemoveEventListenerNs">RemoveEventListenerNs</see> with
		/// arguments which do not identify any currently registered
		/// <see cref="EventListener">EventListener</see> on the EventTarget
		/// has no effect.
		/// </remarks>
		/// <param name="namespaceUri">
		/// Specifies the
		/// <see cref="IEvent.NamespaceUri">IEvent.NamespaceUri</see>
		/// associated with the event for which the user registered the event
		/// listener.
		/// </param>
		/// <param name="type">
		/// Specifies the <see cref="IEvent.Type">IEvent.Type</see> associated
		/// with the event for which the user registered the event listener.
		/// </param>
		/// <param name="listener">
		/// The <see cref="EventListener">EventListener</see> parameter
		/// indicates the <see cref="EventListener">EventListener</see> to
		/// be removed.
		/// </param>
		/// <param name="useCapture">
		/// Specifies whether the
		/// <see cref="EventListener">EventListener</see> being removed was
		/// registered for the capture phase or not. If a listener was
		/// registered twice, once for the capture phase and once for the
		/// target and bubbling phases, each must be removed separately.
		/// Removal of an event listener registered for the capture phase
		/// does not affect the same event listener registered for the target
		/// and bubbling phases, and vice versa.
		/// </param>
		void RemoveEventListenerNs(
			string namespaceUri,
			string type,
			EventListener listener,
			bool useCapture);
		
		/// <summary>
		/// This method allows the DOM application to know if an event
		/// listener, attached to this
		/// <see cref="IEventTarget">IEventTarget</see> or one of its
		/// ancestors, will be triggered by the specified event type during
		/// the dispatch of the event to this event target or one of its
		/// descendants.
		/// </summary>
		/// <param name="namespaceUri">
		/// Specifies the
		/// <see cref="IEvent.NamespaceUri">IEvent.NamespaceUri</see>
		/// associated with the event.
		/// </param>
		/// <param name="type">
		/// Specifies the <see cref="IEvent.Type">IEvent.Type</see>
		/// associated with the event.
		/// </param>
		/// <returns>
		/// <c>true</c> if an event listener will be triggered on the
		/// <see cref="IEventTarget">IEventTarget</see> with the specified
		/// event type, <c>false</c> otherwise.
		/// </returns>
		bool WillTriggerNs(
			string namespaceUri,
			string type);
		
		/// <summary>
		/// This method allows the DOM application to know if this
		/// <see cref="IEventTarget">IEventTarget</see> contains an event
		/// listener registered for the specified event type.
		/// </summary>
		/// <remarks>
		/// This is useful for determining at which nodes within a hierarchy
		/// altered handling of specific event types has been introduced, but
		/// should not be used to determine whether the specified event type
		/// triggers an event listener.
		/// </remarks>
		/// <param name="namespaceUri">
		/// Specifies the
		/// <see cref="IEvent.NamespaceUri">IEvent.NamespaceUri</see>
		/// associated with the event.
		/// </param>
		/// <param name="type">
		/// Specifies the <see cref="IEvent.Type">IEvent.Type</see>
		/// associated with the event.
		/// </param>
		/// <returns>
		/// <c>true</c> if an event listener is registered on this
		/// <see cref="IEventTarget">IEventTarget</see> for the specified
		/// event type, <c>false</c> otherwise.
		/// </returns>
		/// <seealso cref="WillTriggerNs">WillTriggerNs</seealso>
		bool HasEventListenerNs(
			string namespaceUri,
			string type);
		
		#endregion
		
		#endregion
	}
}
