namespace SharpVectors.Dom
{
    /// <summary>
    /// The <see cref="IElement"/> interface represents an element in an HTML or XML document. 
    /// </summary>
    /// <remarks>
    /// Elements may have attributes associated with them; since the <see cref="IElement"/> interface inherits 
    /// from <see cref="INode"/>, the generic <see cref="INode"/> interface attribute <see cref="IAttribute"/> may 
    /// be used to retrieve the set of all attributes for an element. There are 
    /// methods on the <see cref="IElement"/> interface to retrieve either an 
    /// <see cref="IAttribute"/> object by name or an attribute value by name. In XML, 
    /// where an attribute value may contain entity references, an 
    /// <see cref="IAttribute"/> object should be retrieved to examine the possibly 
    /// fairly complex sub-tree representing the attribute value. On the other 
    /// hand, in HTML, where all attributes have simple string values, methods to 
    /// directly access an attribute value can safely be used as a convenience.In 
    /// DOM Level 2, the method <see cref="IXmlNode.Normalize"/> is inherited from the 
    /// <see cref="INode"/> interface where it was moved.
    /// </remarks>
    /// <seealso href="http://www.w3.org/TR/2000/REC-DOM-Level-2-Core-20001113">
    /// Document Object Model (DOM) Level 2 Core Specification</seealso>
	public interface IElement : IXmlElement      
	{
	}
}
