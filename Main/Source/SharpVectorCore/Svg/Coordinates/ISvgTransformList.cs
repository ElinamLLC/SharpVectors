// <developer>niklas@protocol7.com</developer>
// <completed>50</completed>

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// This interface defines a list of SvgTransform objects. 
	/// </summary>
	public interface ISvgTransformList
	{
		 uint NumberOfItems { get; }

		 void Clear();
		 ISvgTransform Initialize(ISvgTransform newItem);
		 ISvgTransform GetItem(uint index);
		 ISvgTransform InsertItemBefore(ISvgTransform newItem, uint index);
		 ISvgTransform ReplaceItem(ISvgTransform newItem, uint index);
		 ISvgTransform RemoveItem(uint index);
		 ISvgTransform AppendItem(ISvgTransform newItem);
		 ISvgTransform CreateSvgTransformFromMatrix(ISvgMatrix matrix);
		 ISvgTransform Consolidate();
	}
}
