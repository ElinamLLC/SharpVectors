using System;

namespace SharpVectors.Dom
{
    //
    // Summary:
    //     Represents an entity declaration, such as <!ENTITY... >.
    public interface IEntity : INode
    {
        //
        // Summary:
        //     Gets the value of the public identifier on the entity declaration.
        //
        // Returns:
        //     The public identifier on the entity. If there is no public identifier, null is
        //     returned.
        string PublicId { get; }
        //
        // Summary:
        //     Gets the value of the system identifier on the entity declaration.
        //
        // Returns:
        //     The system identifier on the entity. If there is no system identifier, null is
        //     returned.
        string SystemId { get; }
        //
        // Summary:
        //     Gets the name of the optional NDATA attribute on the entity declaration.
        //
        // Returns:
        //     The name of the NDATA attribute. If there is no NDATA, null is returned.
        string NotationName { get; }
    }
}
