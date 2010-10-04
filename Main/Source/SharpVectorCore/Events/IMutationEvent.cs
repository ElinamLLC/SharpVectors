using System;

namespace SharpVectors.Dom.Events
{
	/// <summary>
	/// The <see cref="IMutationEvent">IMutationEvent</see> interface
	/// provides specific contextual information associated with
	/// Mutation events.
	/// </summary>
	/// <remarks>
	/// Note: To create an instance of the
	/// <see cref="IMutationEvent">IMutationEvent</see> interface, use the
	/// feature string <c>"MutationEvent"</c> as the value of the input
	/// parameter used with the
	/// <see cref="IDocumentEvent.CreateEvent">IDocumentEvent.CreateEvent</see>
	/// method.
	/// </remarks>
	public interface IMutationEvent
		: IEvent
	{
		/// <summary>
		/// <see cref="RelatedNode">RelatedNode</see> is used to identify a
		/// secondary node related to a mutation event.
		/// </summary>
		/// <remarks>
		/// For example, if a
		/// mutation event is dispatched to a node indicating that its parent
		/// has changed, the <see cref="RelatedNode">RelatedNode</see> is the
		/// changed parent. If an event is instead dispatched to a subtree
		/// indicating a node was changed within it, the 
		/// <see cref="RelatedNode">RelatedNode</see> is the changed node. In
		/// the case of the
		/// <see cref="AttrChangeType.Modification">AttrChangeType.Modification</see>
		/// event it indicates the <see cref="IAttr">IAttr</see> node which
		/// was modified, added, or removed.
		/// </remarks>
		INode RelatedNode
		{
			get;
		}
		
		/// <summary>
		/// <see cref="PrevValue">PrevValue</see> indicates the previous value
		/// of the <see cref="IAttr">IAttr</see> node in
		/// <see cref="AttrChangeType.Modification">AttrChangeType.Modification</see>
		/// events, and of the <see cref="ICharacterData">ICharacterData</see>
		/// node in DOMCharacterDataModified events.
		/// </summary>
		string PrevValue
		{
			get;
		}
		
		/// <summary>
		/// <see cref="NewValue">NewValue</see> indicates the new value of the
		/// <see cref="IAttr">IAttr</see> node in DOMAttrModified events, and
		/// of the <see cref="ICharacterData">ICharacterData</see> node in
		/// DOMCharacterDataModified events.
		/// </summary>
		string NewValue
		{
			get;
		}
		
		/// <summary>
		/// <see cref="AttrName">AttrName</see> indicates the name of the
		/// changed <see cref="IAttr">Attr</see> node in a
		/// <see cref="AttrChangeType.Modification">AttrChangeType.Modification</see>
		/// event.
		/// </summary>
		string AttrName
		{
			get;
		}
		
		/// <summary>
		/// <c>attrChange</c> indicates the type of change which triggered
		/// the DOMAttrModified event. The values can be
		/// <see cref="AttrChangeType.Modification">AttrChangeType.Modification</see>,
		/// <see cref="AttrChangeType.Addition">AttrChangeType.Addition</see>,
		/// or
		/// <see cref="AttrChangeType.Removal">AttrChangeType.Removal</see>.
		/// </summary>
		AttrChangeType AttrChange
		{
			get;
		}
		
		/// <summary>
		/// The <see cref="InitMutationEvent">InitMutationEvent</see> method
		/// is used to initialize the value of a
		/// <see cref="IMutationEvent">IMutationEvent</see> created using the
		/// <see cref="IDocumentEvent.CreateEvent">IDocumentEvent.CreateEvent</see>
		/// method. This method may only be called before the
		/// <see cref="IMutationEvent">IMutationEvent</see> has been
		/// dispatched via the
		/// <see cref="IEventTarget.DispatchEvent">IEventTarget.DispatchEvent</see>
		/// method, though it may be called multiple times before being
		/// dispatched if necessary. If called multiple times, the final
		/// invocation takes precedence.
		/// </summary>
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
		/// <param name="relatedNodeArg">
		/// Specifies the <see cref="IEvent">IEvent</see>'s related Node.
		/// </param>
		/// <param name="prevValueArg">
		/// Specifies the <see cref="IEvent">IEvent</see>'s
		/// <see cref="PrevValue">PrevValue</see> attribute. This value may
		/// be <c>null</c>.
		/// </param>
		/// <param name="newValueArg">
		/// Specifies the <see cref="IEvent">IEvent</see>'s
		/// <see cref="NewValue">NewValue</see> attribute. This value may be
		/// <c>null</c>.
		/// </param>
		/// <param name="attrNameArg">
		/// Specifies the <see cref="IEvent">IEvent</see>'s
		/// <see cref="AttrName">AttrName</see> attribute. This value may be
		/// <c>null</c>.
		/// </param>
		/// <param name="attrChangeArg">
		/// Specifies the <see cref="IEvent">IEvent</see>'s
		/// <see cref="AttrChange">AttrChange</see> attribute.
		/// </param>
		void InitMutationEvent(
			string typeArg,
			bool canBubbleArg,
			bool cancelableArg,
			INode relatedNodeArg,
			string prevValueArg,
			string newValueArg,
			string attrNameArg,
			AttrChangeType attrChangeArg);
		
		/// <summary>
		/// The <see cref="InitMutationEventNs">InitMutationEventNs</see>
		/// method is used to initialize the value of a
		/// <see cref="IMutationEvent">IMutationEvent</see> created using the
		/// <see cref="IDocumentEvent.CreateEvent">IDocumentEvent.CreateEvent</see>
		/// method.
		/// </summary>
		/// <remarks>
		/// This method may only be called before the
		/// <see cref="IMutationEvent">IMutationEvent</see> has been
		/// dispatched via the
		/// <see cref="IEventTarget.DispatchEvent">IEventTarget.DispatchEvent</see>
		/// method, though it may be called multiple times during that phase
		/// if necessary. If called multiple times, the final invocation
		/// takes precedence.
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
		/// <param name="relatedNodeArg">
		/// Specifies the <see cref="IEvent">IEvent</see>'s related Node.
		/// </param>
		/// <param name="prevValueArg">
		/// Specifies the <see cref="IEvent">IEvent</see>'s
		/// <see cref="PrevValue">PrevValue</see> attribute. This value may
		/// be <c>null</c>.
		/// </param>
		/// <param name="newValueArg">
		/// Specifies the <see cref="IEvent">IEvent</see>'s
		/// <see cref="NewValue">NewValue</see> attribute. This value may be
		/// <c>null</c>.
		/// </param>
		/// <param name="attrNameArg">
		/// Specifies the <see cref="IEvent">IEvent</see>'s
		/// <see cref="AttrName">AttrName</see> attribute. This value may be
		/// <c>null</c>.
		/// </param>
		/// <param name="attrChangeArg">
		/// Specifies the <see cref="IEvent">IEvent</see>'s
		/// <see cref="AttrChange">AttrChange</see> attribute.
		/// </param>
		void InitMutationEventNs(
			string namespaceUri,
			string typeArg,
			bool canBubbleArg,
			bool cancelableArg,
			INode relatedNodeArg,
			string prevValueArg,
			string newValueArg,
			string attrNameArg,
			AttrChangeType attrChangeArg);
	}
}
