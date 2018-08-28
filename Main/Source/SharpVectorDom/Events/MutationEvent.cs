using System;

namespace SharpVectors.Dom.Events
{
	/// <summary>
	/// Summary description for MutationEvent.
	/// </summary>
	public class MutationEvent : Event, IMutationEvent
	{
		#region Private Fields
		
		private INode _relatedNode;
		private string _prevValue;
		private string _newValue;
		private string _attrName;
		private AttrChangeType _attrChange;
		
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
				return _relatedNode;
			}
		}
		
		public string PrevValue
		{
			get
			{
				return _prevValue;
			}
		}
		
		public string NewValue
		{
			get
			{
				return _newValue;
			}
		}
		
		public string AttrName
		{
			get
			{
				return _attrName;
			}
		}
		
		public AttrChangeType AttrChange
		{
			get
			{
				return _attrChange;
			}
		}
		
		public void InitMutationEvent(string eventType, bool bubbles, bool cancelable, INode relatedNode,
			string prevValue, string newValue, string attrName, AttrChangeType attrChange)
		{
			InitEvent(eventType, bubbles, cancelable);
			
			_relatedNode = relatedNode;
			_prevValue   = prevValue;
			_newValue    = newValue;
			_attrName    = attrName;
			_attrChange  = attrChange;
		}
		
		public void InitMutationEventNs(string namespaceUri, string eventType, bool bubbles, bool cancelable,
			INode relatedNode, string prevValue, string newValue, string attrName, AttrChangeType attrChange)
		{
			InitEventNs(namespaceUri, eventType, bubbles, cancelable);
			
			_relatedNode = relatedNode;
			_prevValue   = prevValue;
			_newValue    = newValue;
			_attrName    = attrName;
			_attrChange  = attrChange;
		}
		
		#endregion
	}
}
