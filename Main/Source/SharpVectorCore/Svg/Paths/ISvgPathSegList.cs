// <developer>niklas@protocol7.com</developer>
// <completed>90</completed>

using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// This interface defines a list of SvgPathSeg objects. 
	/// </summary>
	public interface ISvgPathSegList
	{
		int NumberOfItems { get; }
		void Clear();
		ISvgPathSeg Initialize(ISvgPathSeg newItem);
		ISvgPathSeg GetItem(int index);
		ISvgPathSeg InsertItemBefore(ISvgPathSeg newItem, int index);
		ISvgPathSeg ReplaceItem(ISvgPathSeg newItem, int index);
		ISvgPathSeg RemoveItem(int index);
		ISvgPathSeg AppendItem(ISvgPathSeg newItem);
	}
}

