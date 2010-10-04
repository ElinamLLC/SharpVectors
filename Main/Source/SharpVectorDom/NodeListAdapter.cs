using System;
using System.Xml;

namespace SharpVectors.Dom
{
	/// <summary>
	/// Summary description for NodeListAdapter.
	/// </summary>
	public class NodeListAdapter
		: INodeList
	{
		#region Private Fields
		
		XmlNodeList nodeList;
		
		#endregion
		
		#region Constructors
		
		public NodeListAdapter(
			XmlNodeList nodeList)
		{
			this.nodeList = nodeList;
		}
		
		#endregion
		
		#region INodeList interface
		
		public INode this[
			ulong index]
		{
			get
			{
				return (INode)nodeList[(int)index];
			}
		}
		
		public ulong Count
		{
			get
			{
				return (ulong)nodeList.Count;
			}
		}
		
		#endregion
	}
}
