using System;

namespace SharpVectors.Dom.Events
{
	/// <summary>
	/// The <see cref="ICustomEvent">ICustomEvent</see> interface gives access
	/// to the attributes
	/// <see cref="IEvent.CurrentTarget">IEvent.CurrentTarget</see> and
	/// <see cref="IEvent.EventPhase">IEvent.EventPhase</see>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// It is intended
	/// to be used by the DOM Events implementation to access the underlying
	/// current target and event phase while dispatching a custom
	/// <see cref="IEvent">IEvent</see> in the tree; it is also intended to be
	/// implemented, and not used, by DOM applications.
	/// </para>
	/// <para>
	/// The methods contained in this interface are not intended to be used by
	/// a DOM application, especially during the dispatch on the Event object.
	/// Changing the current target or the current phase may conduct into
	/// unpredictable results of the event flow. The DOM Events implementation
	/// should ensure that both methods return the appropriate current target
	/// and phase before invoking each event listener on the current target to
	/// protect DOM applications from malicious event listeners.
	/// </para>
	/// <para>
	/// Note: If this interface is supported by the event object,
	/// <see cref="IEvent.IsCustom">IEvent.IsCustom</see> must return
	/// <c>true</c>.
	/// </para>
	/// </remarks>
	public interface ICustomEvent
		: IEvent
	{
		#region Properties
		
		#region DOM Level 3 Experimental
		
		/// <summary>
		/// This method will return <c>true</c> if the method
		/// <see cref="IEvent.StopPropagation">IEvent.StopPropagation</see> has been called
		/// for this event, <c>false</c> in any other cases. 
		/// </summary>
		/// <value>
		/// <c>true</c> if the event propagation has been stopped in the
		/// current group.
		/// </value>
		bool IsPropagationStopped
		{
			get;
		}
		
		/// <summary>
		/// The
		/// <see cref="IsImmediatePropagationStopped">IsImmediatePropagationStopped</see>
		/// method is used by the DOM Events implementation to know if the
		/// method
		/// <see cref="IEvent.StopImmediatePropagation">IEvent.StopImmediatePropagation</see>
		/// has been called for this event. It returns <c>true</c> if the
		/// method has been called, <c>false</c> otherwise. 
		/// </summary>
		bool IsImmediatePropagationStopped
		{
			get;
		}
		
		#endregion
		
		#endregion
		
		#region Methods
		
		#region DOM Level 3 Experimental
		
		/// <summary>
		/// The <see cref="SetDispatchState">SetDispatchState</see> method is
		/// used by the DOM Events implementation to set the values of
		/// <see cref="IEvent.CurrentTarget">IEvent.CurrentTarget</see> and
		/// <see cref="IEvent.EventPhase">IEvent.EventPhase</see>.
		/// </summary>
		/// <remarks>
		/// It also reset the states of
		/// <see cref="IsPropagationStopped">IsPropagationStopped</see> and
		/// <see cref="IsImmediatePropagationStopped">IsImmediatePropagationStopped</see>.
		/// </remarks>
		/// <param name="target">
		/// Specifies the new value for the
		/// <see cref="IEvent.CurrentTarget">IEvent.CurrentTarget</see>
		/// attribute.
		/// </param>
		/// <param name="phase">
		/// Specifies the new value for the
		/// <see cref="IEvent.EventPhase">IEvent.EventPhase</see> attribute.
		/// </param>
		void SetDispatchState(
			IEventTarget target,
			ushort phase);
		
		#endregion
		
		#endregion
	}
}
