using System;

namespace SharpVectors.Dom.Events
{
	/// <summary>
	/// An integer indicating the type of error generated.
	/// </summary>
	public enum EventExceptionCode
	{
		/// <summary>
		/// If the <see cref="IEvent.Type">IEvent.Type</see> was not specified
		/// by initializing the event before the method was called.
		/// Specification of the <see cref="IEvent.Type">IEvent.Type</see> as
		/// <c>null</c> or an empty string will also trigger this exception.
		/// </summary>
		UnspecifiedEventTypeErr,
		
		/// <summary>
		/// If the <see cref="IEvent">IEvent</see> object is already
		/// dispatched in the tree.
		/// </summary>
		DispatchRequestErr,
	}
}
