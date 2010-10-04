// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>
// <completed>60</completed>

using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// This interface defines a list of SvgPoint objects. 
	/// </summary>
	public interface ISvgPointList
	{
		uint NumberOfItems{get;}

		void Clear();
		ISvgPoint Initialize(ISvgPoint newItem);
		ISvgPoint GetItem(uint index);
		ISvgPoint InsertItemBefore(ISvgPoint newItem, uint index);
		ISvgPoint ReplaceItem(ISvgPoint newItem, uint index);
		ISvgPoint RemoveItem(uint index);
		ISvgPoint AppendItem(ISvgPoint newItem);
                        
        // not part of the SVG spec
        void FromString(string listString);
	}
}
