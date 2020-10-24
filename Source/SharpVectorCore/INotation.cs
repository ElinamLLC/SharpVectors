using System;

namespace SharpVectors.Dom
{
    /// <summary>
    /// This interface represents a notation declared in the DTD. A notation either 
    /// declares, by name, the format of an unparsed entity (see section 4.7 of 
    /// the XML 1.0 specification ), or is used for formal declaration of 
    /// processing instruction targets (see section 2.6 of the XML 1.0 
    /// specification ). The <see cref="INode.NodeName"/> attribute inherited from 
    /// <see cref="INode"/> is set to the declared name of the notation.
    /// <para>The DOM Level 1 does not support editing <see cref="INotation"/> nodes; 
    /// they are therefore readonly.
    /// </para>
    /// <para>A <see cref="INotation"/> node does not have any parent.</para>
    /// </summary>
    /// <seealso href="http://www.w3.org/TR/2000/REC-DOM-Level-2-Core-20001113">
    /// Document Object Model (DOM) Level 2 Core Specification</seealso>
    public interface INotation : INode
    {
        /// <summary>
        /// <para>Gets the value of the public identifier on the notation declaration.</para>
        /// If the public identifier was not specified, this is <see langword="null"/>.
        /// </summary>
        /// <value>
        /// The public identifier on the notation. If there is no public identifier, <see langword="null"/>
        /// is returned.
        /// </value>
        string PublicId { get; }

        /// <summary>
        /// Gets the value of the system identifier on the notation declaration. If the system identifier was 
        /// not specified, this is <see langword="null"/>.
        /// </summary>
        /// <value>
        /// The system identifier on the notation. If there is no system identifier, <see langword="null"/>
        /// is returned.
        /// </value>
        string SystemId { get; }
    }
}
