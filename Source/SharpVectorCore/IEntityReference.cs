namespace SharpVectors.Dom
{
    /// <summary>
    /// <see cref="IEntityReference"/> objects may be inserted into the structure model when an entity reference 
    /// is in the source document, or when the user wishes to insert an entity reference. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// Note that character references and references to predefined entities are considered to be expanded by 
    /// the HTML or XML processor so that characters are represented by their Unicode equivalent rather than 
    /// by an entity reference. Moreover, the XML processor may completely expand references to entities while 
    /// building the structure model, instead of providing <see cref="IEntityReference"/> objects. 
    /// </para>
    /// <para>
    /// If it does provide such objects, then for a given <see cref="IEntityReference"/> node, it may be that there is no 
    /// <see cref="IEntity"/> node representing the referenced entity. If such an <see cref="IEntity"/> exists, then 
    /// the subtree of the <see cref="IEntityReference"/> node is in general a copy of the <see cref="IEntity"/> node subtree. 
    /// </para>
    /// However, this may not be true when an entity contains an unbound namespace prefix. In such a case, because the 
    /// namespace prefix resolution depends on where the entity reference is, the descendants of the 
    /// <see cref="IEntityReference"/> node may be bound to different namespace URIs.
    /// <para>As for <see cref="IEntity"/> nodes, <see cref="IEntityReference"/> nodes and 
    /// all their descendants are readonly.
    /// </para>
    /// </remarks>
    /// <seealso href="http://www.w3.org/TR/2000/REC-DOM-Level-2-Core-20001113">
    /// Document Object Model (DOM) Level 2 Core Specification</seealso>
	public interface IEntityReference : INode
	{
	}
}
