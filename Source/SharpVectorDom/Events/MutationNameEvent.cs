using System;

namespace SharpVectors.Dom.Events
{
	/// <summary>
	/// Summary description for MutationNameEvent.
	/// </summary>
	public class MutationNameEvent: MutationEvent, IMutationNameEvent
	{
		#region Private Fields
		
		private string _prevNamespaceUri;
		private string _prevNodeName;
		
		#endregion
		
		#region IMutationEvent interface
		
		public string PrevNamespaceUri
		{
			get
			{
				return _prevNamespaceUri;
			}
		}
		
		public string PrevNodeName
		{
			get
			{
				return _prevNodeName;
			}
		}
		
		public void InitMutationNameEvent(string eventType, bool bubbles, bool cancelable,
			INode relatedNode, string prevNamespaceUri, string prevNodeName)
		{
			InitMutationEvent(eventType, bubbles, cancelable, relatedNode, string.Empty, string.Empty, string.Empty, AttrChangeType.None);
			
			_prevNamespaceUri = prevNamespaceUri;
			_prevNodeName     = prevNodeName;
		}
		
		public void InitMutationNameEventNs(string namespaceUri, string eventType, bool bubbles, 
			bool cancelable, INode relatedNode, string prevNamespaceUri, string prevNodeName)
		{
			InitMutationEventNs(namespaceUri, eventType, bubbles, cancelable,
				relatedNode, string.Empty, string.Empty, string.Empty, AttrChangeType.None);
			
			_prevNamespaceUri = prevNamespaceUri;
			_prevNodeName     = prevNodeName;
		}
		
		#endregion
	}
}
