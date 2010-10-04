using System;

namespace SharpVectors.Dom
{
	/// <summary>
	/// Summary description for INodeList.
	/// </summary>
	public interface INodeList
	{
		INode this[ulong index]
		{
			get;
		}
		
		ulong Count
		{
			get;
		}
	}
}
