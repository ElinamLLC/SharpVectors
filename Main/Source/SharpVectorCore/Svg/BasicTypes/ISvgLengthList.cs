namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// This interface defines a list of SvgLength objects. 
	/// </summary>
	/// <developer>niklas@protocol7.com</developer>
	/// <completed>20</completed>
	public interface ISvgLengthList
	{
		uint NumberOfItems{get;}
		void Clear();
		ISvgLength Initialize(ISvgLength newItem);
		ISvgLength GetItem(uint index);
		ISvgLength InsertItemBefore(ISvgLength newItem, uint index);
		ISvgLength ReplaceItem(ISvgLength newItem, uint index);
		ISvgLength RemoveItem(uint index);
		ISvgLength AppendItem(ISvgLength newItem);
                        
        // not part of the SVG spec
        void FromString(string listString);
	}
}
