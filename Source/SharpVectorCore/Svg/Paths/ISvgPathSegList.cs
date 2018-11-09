using System;
using System.Collections.Generic;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// This interface defines a list of SvgPathSeg objects. 
	/// </summary>
	public interface ISvgPathSegList : IList<ISvgPathSeg>
	{
		int NumberOfItems { get; }

		ISvgPathSeg Initialize(ISvgPathSeg newItem);
		ISvgPathSeg GetItem(int index);
		ISvgPathSeg InsertItemBefore(ISvgPathSeg newItem, int index);
		ISvgPathSeg ReplaceItem(ISvgPathSeg newItem, int index);
		ISvgPathSeg RemoveItem(int index);
		ISvgPathSeg AppendItem(ISvgPathSeg newItem);
	}
}

