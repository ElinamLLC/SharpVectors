using System;

namespace SharpVectors.Dom.Events
{
	/// <summary>
	/// The <see cref="EventListener">EventListener</see> delegate is the
	/// primary way for handling events.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Users register their event listener on an
	/// <see cref="IEventTarget">IEventTarget</see>. The users should also
	/// remove their <see cref="EventListener">EventListener</see> from its
	/// <see cref="IEventTarget">IEventTarget</see> after they have completed
	/// using the listener.
	/// </para>
	/// <para>
	/// Copying a <see cref="INode">INode</see> does not copy the event
	/// listeners attached to it. Event listeners must be attached to the
	/// newly created <see cref="INode">INode</see> afterwards if so desired.
	/// Therefore, <see cref="INode">INode</see>s are copied using
	/// <see cref="INode.CloneNode">INode.CloneNode</see> or
	/// <see cref="IRange.CloneContents">IRange.CloneContents</see>, the
	/// <see cref="EventListener">EventListener</see>s attached to the
	/// source <see cref="INode">INode</see>s are not attached to their copies.
	/// </para>
	/// <para>
	/// Moving a <see cref="INode">INode</see> does not affect the event
	/// listeners attached to it. Therefore, when
	/// <see cref="INode">INode</see>s are moved using
	/// <see cref="IDocument.AdoptNode">IDocument.AdoptNode</see>,
	/// <see cref="INode.AppendChild">INode.AppendChild</see>, or
	/// <see cref="IRange.ExtractContents">IRange.ExtractContents</see>, the
	/// <see cref="EventListener">EventListener</see>s attached to the
	/// moved <see cref="INode">INode</see>s stay attached to them.
	/// </para>
	/// </remarks>
	/// <paramref name="e">
	/// The <see cref="IEvent">IEvent</see> contains contextual information
	/// about the
	/// <see href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/glossary.html#dt-event">event</see>.
	/// </paramref>
	public delegate void EventListener(
		IEvent e);
}
