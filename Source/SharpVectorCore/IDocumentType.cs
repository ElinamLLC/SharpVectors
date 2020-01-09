using System.Xml;

namespace SharpVectors.Dom
{
	/// <summary>
	/// Summary description for IDocumentType.
	/// </summary>
	public interface IDocumentType : INode
	{
        //
        // Summary:
        //     Gets the collection of System.Xml.XmlEntity nodes declared in the document type
        //     declaration.
        //
        // Returns:
        //     An System.Xml.XmlNamedNodeMap containing the XmlEntity nodes. The returned XmlNamedNodeMap
        //     is read-only.
        XmlNamedNodeMap Entities { get; }
        //
        // Summary:
        //     Gets the collection of System.Xml.XmlNotation nodes present in the document type
        //     declaration.
        //
        // Returns:
        //     An System.Xml.XmlNamedNodeMap containing the XmlNotation nodes. The returned
        //     XmlNamedNodeMap is read-only.
        XmlNamedNodeMap Notations { get; }
        //
        // Summary:
        //     Gets the value of the public identifier on the DOCTYPE declaration.
        //
        // Returns:
        //     The public identifier on the DOCTYPE. If there is no public identifier, null
        //     is returned.
        string PublicId { get; }
        //
        // Summary:
        //     Gets the value of the system identifier on the DOCTYPE declaration.
        //
        // Returns:
        //     The system identifier on the DOCTYPE. If there is no system identifier, null
        //     is returned.
        string SystemId { get; }
        //
        // Summary:
        //     Gets the value of the document type definition (DTD) internal subset on the DOCTYPE
        //     declaration.
        //
        // Returns:
        //     The DTD internal subset on the DOCTYPE. If there is no DTD internal subset, String.Empty
        //     is returned.
        string InternalSubset { get; }
    }
}
