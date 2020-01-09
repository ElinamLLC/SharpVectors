using System;

namespace SharpVectors.Dom
{
    public interface INotation : INode
    {
        //
        // Summary:
        //     Gets the value of the public identifier on the notation declaration.
        //
        // Returns:
        //     The public identifier on the notation. If there is no public identifier, null
        //     is returned.
        string PublicId { get; }

        //
        // Summary:
        //     Gets the value of the system identifier on the notation declaration.
        //
        // Returns:
        //     The system identifier on the notation. If there is no system identifier, null
        //     is returned.
        string SystemId { get; }
    }
}
