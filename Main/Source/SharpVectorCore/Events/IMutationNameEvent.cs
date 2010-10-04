using System;

namespace SharpVectors.Dom.Events
{
	/// <summary>
	/// The <see cref="IMutationNameEvent">IMutationNameEvent</see> interface
	/// provides specific contextual information associated with Mutation
	/// name event types.
	/// </summary>
	/// <remarks>
	/// Note: To create an instance of the
	/// <see cref="IMutationNameEvent">IMutationNameEvent</see> interface,
	/// use the feature string <c>"MutationNameEvent"</c> as the value of the
	/// input parameter used with the
	/// <see cref="CreateEvent">CreateEvent</see> method of the
	/// <see cref="IDocumentEvent">IDocumentEvent</see> interface.
	/// </remarks>
	public interface IMutationNameEvent
	{
		/// <summary>
		/// The previous value of the
		/// <see cref="RelatedNode">RelatedNode</see>'s namespace URI.
		/// </summary>
		string PrevNamespaceUri
		{
			get;
		}
		
		/// <summary>
		/// The previous value of the
		/// <see cref="RelatedNode">RelatedNode</see>'s nodeName.
		/// </summary>
		string PrevNodeName
		{
			get;
		}
		
		/// <summary>
		/// The <see cref="InitMutationNameEvent">InitMutationNameEvent</see>
		/// method is used to initialize the value of a
		/// <see cref="IMutationNameEvent">IMutationNameEvent</see> created
		/// using the
		/// <see cref="IDocumentEvent.CreateEvent">IDocumentEvent.CreateEvent</see>
		/// method. This method may only be called before the
		/// <see cref="IMutationNameEvent">IMutationNameEvent</see> has been
		/// dispatched via the
		/// <see cref="IEventTarget.DispatchEvent">IEventTarget.DispatchEvent</see>
		/// method, though it may be called multiple times during that phase
		/// if necessary. If called multiple times, the final invocation
		/// takes precedence.
		/// </summary>
		/// <param name="typeArg">
		/// Specifies the event type.
		/// </param>
		/// <param name="canBubbleArg">
		/// Specifies whether or not the event can bubble.
		/// </param>
		/// <param name="cancelableArg">
		/// Specifies whether or not the event's default action can be prevented.
		/// </param>
		/// <param name="relatedNodeArg">
		/// Specifies the <see cref="IEvent">IEvent</see>'s related Node.
		/// </param>
		/// <param name="prevNamespaceUri">
		/// Specifies the previous <see cref="NamespaceUri">NamespaceUri</see>
		/// of the related <see cref="INode">INode</see>. This value may be
		/// <c>null</c>.
		/// </param>
		/// <param name="prevNodeName">
		/// Specifies the previous <see cref="NodeName">NodeName</see> of the
		/// related Node.
		/// </param>
		void InitMutationNameEvent(
			string typeArg,
			bool canBubbleArg,
			bool cancelableArg,
			INode relatedNodeArg,
			string prevNamespaceUri,
			string prevNodeName);
		
		/// <summary>
		/// The
		/// <see cref="InitMutationNameEventNs">InitMutationNameEventNs</see>
		/// method is used to initialize the value of a
		/// <see cref="IMutationNameEvent">IMutationNameEvent</see> created
		/// using the
		/// <see cref="IDocumentEvent.CreateEvent">IDocumentEvent.CreateEvent</see>
		/// method. This method may only be called before the
		/// <see cref="IMutationNameEvent">IMutationNameEvent</see> has been
		/// dispatched via the
		/// <see cref="IEventTarget.DispatchEvent">IEventTarget.DispatchEvent</see>
		/// method, though it may be called multiple times during that phase if necessary. If called multiple times, the final invocation takes precedence. 
		/// </summary>
		/// <param name="namespaceUri">
		/// Specifies the
		/// <see cref="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/glossary.html#dt-namespaceURI">namespace URI</see>
		/// associated with this event, or <c>null</c> if the application
		/// wish to have no namespace.
		/// </param>
		/// <param name="typeArg">
		/// Specifies the event type.
		/// </param>
		/// <param name="canBubbleArg">
		/// Specifies whether or not the event can bubble.
		/// </param>
		/// <param name="cancelableArg">
		/// Specifies whether or not the event's default action can be prevented.
		/// </param>
		/// <param name="relatedNodeArg">
		/// Specifies the <see cref="IEvent">IEvent</see>'s related Node.
		/// </param>
		/// <param name="prevNamespaceUri">
		/// Specifies the previous
		/// <see cref="INode.NamespaceUri">NamespaceUri</see> of the related
		/// Node. This value may be <c>null</c>.
		/// </param>
		/// <param name="prevNodeName">
		/// Specifies the previous <see cref="INode.NodeName">NodeName</see>
		/// of the related Node.
		/// </param>
		void InitMutationNameEventNs(
			string namespaceUri,
			string typeArg,
			bool canBubbleArg,
			bool cancelableArg,
			INode relatedNodeArg,
			string prevNamespaceUri,
			string prevNodeName);
	}
}
