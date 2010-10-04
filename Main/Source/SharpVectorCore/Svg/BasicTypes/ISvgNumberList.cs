namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// This interface defines a list of SvgNumber objects. 
	/// </summary>
	/// <developer>niklas@protocol7.com</developer>
	/// <developer>kevin@kevlindev.com</developer>
	/// <completed>100</completed>
	public interface ISvgNumberList
	{
		uint NumberOfItems{get;}
		void Clear();
		ISvgNumber Initialize(ISvgNumber newItem);
		ISvgNumber GetItem(uint index);
		ISvgNumber InsertItemBefore(ISvgNumber newItem, uint index);
		ISvgNumber ReplaceItem(ISvgNumber newItem, uint index);
		ISvgNumber RemoveItem(uint index);
		ISvgNumber AppendItem(ISvgNumber newItem);
                
        // not part of the SVG spec
        void FromString(string listString);
	}
}
