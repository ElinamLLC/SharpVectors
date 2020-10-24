using System.Xml;

namespace SharpVectors.Dom
{
    /// <summary>
    /// Represents the text content of an element or attribute.
    /// </summary>
    public interface IText : ICharacterData
	{
        /// <summary>
        /// Splits the node into two nodes at the specified offset, keeping both in the tree as siblings.
        /// </summary>
        /// <param name="offset">The offset at which to split the node.</param>
        /// <returns>The new node.</returns>
        XmlText SplitText(int offset);
    }
}
