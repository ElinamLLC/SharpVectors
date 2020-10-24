using System;

namespace SharpVectors.Dom
{
    /// <summary>
    /// This interface represents an entity, either parsed or unparsed, in an XML 
    /// document. Note that this models the entity itself not the entity 
    /// declaration. <see cref="IEntity"/> declaration modeling has been left for a 
    /// later Level of the DOM specification.
    /// <para>
    /// Represents an entity declaration, such as <!ENTITY... >.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>The <see cref="INode.NodeName"/> attribute that is inherited from 
    /// <see cref="INode"/> contains the name of the entity.
    /// </para>
    /// <para>An XML processor may choose to completely expand entities before the 
    /// structure model is passed to the DOM; in this case there will be no 
    /// <see cref="IEntityReference"/> nodes in the document tree.
    /// </para>
    /// <para>XML does not mandate that a non-validating XML processor read and 
    /// process entity declarations made in the external subset or declared in 
    /// external parameter entities. This means that parsed entities declared in 
    /// the external subset need not be expanded by some classes of applications, 
    /// and that the replacement value of the entity may not be available. When 
    /// the replacement value is available, the corresponding <see cref="IEntity"/> 
    /// node's child list represents the structure of that replacement text. 
    /// Otherwise, the child list is empty.
    /// </para>
    /// <para>The DOM Level 2 does not support editing <see cref="IEntity"/> nodes; if a 
    /// user wants to make changes to the contents of an <see cref="IEntity"/>, 
    /// every related <see cref="IEntityReference"/> node has to be replaced in the 
    /// structure model by a clone of the <see cref="IEntity"/>'s contents, and 
    /// then the desired changes must be made to each of those clones instead. 
    /// <see cref="IEntity"/> nodes and all their descendants are readonly.
    /// </para>
    /// <para>An <see cref="IEntity"/> node does not have any parent. If the entity 
    /// contains an unbound namespace prefix, the <see cref="INode.NamespaceURI"/> of 
    /// the corresponding node in the <see cref="IEntity"/> node subtree is 
    /// <see langword="null"/>. The same is true for <see cref="IEntityReference"/> 
    /// nodes that refer to this entity, when they are created using the 
    /// <see cref="IDocument.CreateEntityReference"/> method of the <see cref="IDocument"/> 
    /// interface. The DOM Level 2 does not support any mechanism to resolve 
    /// namespace prefixes.
    /// </para>
    /// </remarks>
    /// <seealso href="http://www.w3.org/TR/2000/REC-DOM-Level-2-Core-20001113">
    /// Document Object Model (DOM) Level 2 Core Specification</seealso>
    public interface IEntity : INode
    {
        /// <summary>
        /// Gets the public identifier associated with the entity, if specified. If the 
        /// public identifier was not specified, this is <see langword="null"/>.
        /// </summary>
        /// <value>
        /// The public identifier on the entity. If there is no public identifier, <see langword="null"/> is returned.
        /// </value>
        string PublicId { get; }

        /// <summary>
        /// Gets the value of system identifier associated with the entity declaration, if specified. If the 
        /// system identifier was not specified, this is <see langword="null"/>.
        /// </summary>
        /// <value>
        /// The system identifier on the entity. If there is no system identifier, <see langword="null"/> is
        /// returned.
        /// </value>
        string SystemId { get; }

        /// <summary>
        /// <para>Gets the name of the optional NDATA attribute on the entity declaration.</para>
        /// For unparsed entities, the name of the notation for the entity. For 
        /// parsed entities, this is <see langword="null"/>. 
        /// </summary>
        /// <value>
        /// The name of the NDATA attribute. If there is no NDATA, <see langword="null"/> is returned.
        /// </value>
        string NotationName { get; }
    }
}
