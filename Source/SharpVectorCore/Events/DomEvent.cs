using SharpVectors.Dom;

namespace SharpVectors.Dom.Events
{
	/// <summary>
	/// The <see cref="EventListener">EventListener</see> delegate is the
	/// primary way for handling events.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Users register their event listener on an <see cref="IEventTarget">IEventTarget</see>. The users should also
	/// remove their <see cref="EventListener">EventListener</see> from its <see cref="IEventTarget">IEventTarget</see> 
	/// after they have completed using the listener.
	/// </para>
	/// <para>
	/// Copying a <see cref="INode">INode</see> does not copy the event listeners attached to it. Event listeners 
	/// must be attached to the newly created <see cref="INode">INode</see> afterwards if so desired.
	/// </para>
	/// </remarks>
	/// <paramref name="e">
	/// The <see cref="IEvent">IEvent</see> contains contextual information
	/// about the <see href="https://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/glossary.html#dt-event">event</see>.
	/// </paramref>
	public delegate void EventListener(IEvent e);
}
