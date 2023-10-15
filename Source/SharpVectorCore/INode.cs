namespace SharpVectors.Dom
{
    /// <summary>
    /// The <see cref="INode"/> interface is the primary datatype for the entire Document Object Model. 
    /// It represents a single node in the document tree. 
    /// </summary>
    /// <remarks>
    /// While all objects implementing the <see cref="INode"/> interface expose methods for dealing with children, 
    /// not all objects implementing the <see cref="INode"/> interface may have children. For example, <see cref="IText"/> 
    /// nodes may not have children, and adding children to such nodes results in a <see cref="DomException"/> being raised.
    /// <para>The attributes <see cref="IXmlNode.Name"/>, <see cref="IXmlNode.Value"/> and <see cref="IXmlNode.Attributes"/> are included 
    /// as a mechanism to get at node information without casting down to the specific derived interface. 
    /// In cases where there is no obvious mapping of these attributes for a specific <see cref="IXmlNode.NodeType"/> 
    /// (e.g., <see cref="IXmlNode.Value"/> for an <see cref="IElement"/> or <see cref="IXmlNode.Attributes"/> for a <see cref="IComment"/>), 
    /// this returns <see langword="null"/>. Note that the specialized interfaces may contain additional and more 
    /// convenient mechanisms to get and set the relevant information.
    /// </para>
    /// </remarks>
    /// <seealso href="https://www.w3.org/TR/2000/REC-DOM-Level-2-Core-20001113">
    /// Document Object Model (DOM) Level 2 Core Specification</seealso>
	public interface INode : IXmlNode
	{
	}
}
