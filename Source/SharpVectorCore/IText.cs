using System.Xml;

namespace SharpVectors.Dom
{
	/// <summary>
	/// Summary description for IText.
	/// </summary>
	public interface IText : ICharacterData
	{
        //
        // Summary:
        //     Splits the node into two nodes at the specified offset, keeping both in the tree
        //     as siblings.
        //
        // Parameters:
        //   offset:
        //     The offset at which to split the node.
        //
        // Returns:
        //     The new node.
        XmlText SplitText(int offset);
    }
}
