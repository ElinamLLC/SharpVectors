using System;

namespace SharpVectors.Dom.Events
{
	/// <summary>
	/// Summary description for MutationEvent.
	/// </summary>
	public class MutationEvent : Event, IMutationEvent
	{
		#region Private Fields
		
		private INode relatedNode;
		private string prevValue;
		private string newValue;
		private string attrName;
		private AttrChangeType attrChange;
		
		#endregion
		
		#region Constructors
		
		public MutationEvent()
		{
		}
		
		#endregion
		
		#region IMutationEvent interface
		
		public INode RelatedNode
		{
			get
			{
				return relatedNode;
			}
		}
		
		public string PrevValue
		{
			get
			{
				return prevValue;
			}
		}
		
		public string NewValue
		{
			get
			{
				return newValue;
			}
		}
		
		public string AttrName
		{
			get
			{
				return attrName;
			}
		}
		
		public AttrChangeType AttrChange
		{
			get
			{
				return attrChange;
			}
		}
		
		public void InitMutationEvent(
			string eventType,
			bool bubbles,
			bool cancelable,
			INode relatedNode,
			string prevValue,
			string newValue,
			string attrName,
			AttrChangeType attrChange)
		{
			InitEvent(eventType, bubbles, cancelable);
			
			this.relatedNode = relatedNode;
			this.prevValue = prevValue;
			this.newValue = newValue;
			this.attrName = attrName;
			this.attrChange = attrChange;
		}
		
		public void InitMutationEventNs(
			string namespaceUri,
			string eventType,
			bool bubbles,
			bool cancelable,
			INode relatedNode,
			string prevValue,
			string newValue,
			string attrName,
			AttrChangeType attrChange)
		{
			InitEventNs(namespaceUri, eventType, bubbles, cancelable);
			
			this.relatedNode = relatedNode;
			this.prevValue = prevValue;
			this.newValue = newValue;
			this.attrName = attrName;
			this.attrChange = attrChange;
		}
		
		#endregion
	}
}
