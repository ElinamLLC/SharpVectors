using System;

namespace SharpVectors.Dom.Events
{
	/// <summary>
	/// Summary description for MutationNameEvent.
	/// </summary>
	public class MutationNameEvent: MutationEvent, IMutationNameEvent
	{
		#region Private Fields
		
		private string prevNamespaceUri;
		private string prevNodeName;
		
		#endregion
		
		#region IMutationEvent interface
		
		public string PrevNamespaceUri
		{
			get
			{
				return prevNamespaceUri;
			}
		}
		
		public string PrevNodeName
		{
			get
			{
				return prevNodeName;
			}
		}
		
		public void InitMutationNameEvent(
			string eventType,
			bool bubbles,
			bool cancelable,
			INode relatedNode,
			string prevNamespaceUri,
			string prevNodeName)
		{
			InitMutationEvent(
				eventType, bubbles, cancelable,
				relatedNode, "", "", "", AttrChangeType.None);
			
			this.prevNamespaceUri = prevNamespaceUri;
			this.prevNodeName = prevNodeName;
		}
		
		public void InitMutationNameEventNs(
			string namespaceUri,
			string eventType,
			bool bubbles,
			bool cancelable,
			INode relatedNode,
			string prevNamespaceUri,
			string prevNodeName)
		{
			InitMutationEventNs(
				namespaceUri, eventType, bubbles, cancelable,
				relatedNode, "", "", "", AttrChangeType.None);
			
			this.prevNamespaceUri = prevNamespaceUri;
			this.prevNodeName = prevNodeName;
		}
		
		#endregion
	}
}
