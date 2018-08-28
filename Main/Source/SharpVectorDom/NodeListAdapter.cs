using System;
using System.Xml;

namespace SharpVectors.Dom
{
	/// <summary>
	/// Summary description for NodeListAdapter.
	/// </summary>
	public class NodeListAdapter : INodeList
	{
		#region Private Fields
		
		private XmlNodeList _nodeList;
		
		#endregion
		
		#region Constructors
		
		public NodeListAdapter(XmlNodeList nodeList)
		{
			_nodeList = nodeList;
		}
		
		#endregion
		
		#region INodeList interface
		
		public INode this[ulong index]
		{
			get
			{
				return (INode)_nodeList[(int)index];
			}
		}
		
		public ulong Count
		{
			get
			{
				return (ulong)_nodeList.Count;
			}
		}
		
		#endregion
	}
}
