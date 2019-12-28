namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// This interface defines a list of SvgNumber objects. 
    /// </summary>
    public interface ISvgNumberList
    {
        int Count { get; }
        ISvgNumber this[int index] { get; }

        uint NumberOfItems { get; }
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
