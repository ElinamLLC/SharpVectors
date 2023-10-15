using System;
using System.Xml;

namespace SharpVectors.Dom
{
    /// <summary>
    /// This represents the document type declaration.
    /// </summary>
    /// <seealso cref="XmlDocumentType"/>
    public interface IDocumentType : INode
	{
        /// <summary>
        /// Gets the collection of <see cref="XmlEntity"/> nodes declared in the document type declaration.
        /// </summary>
        /// <value>
        /// An <see cref="XmlNamedNodeMap"/> containing the <see cref="XmlEntity"/> nodes. 
        /// The returned <see cref="XmlNamedNodeMap"/> is read-only.
        /// </value>
        XmlNamedNodeMap Entities { get; }

        ///<summary>
        /// Gets the collection of <see cref="XmlNotation"/> nodes present in the document type declaration.
        ///</summary>
        ///<value>
        /// An <see cref="XmlNamedNodeMap"/> containing the <see cref="XmlNotation"/> nodes. The returned
        /// <see cref="XmlNamedNodeMap"/> is read-only.
        ///</value>
        XmlNamedNodeMap Notations { get; }

        ///<summary>
        /// Gets the value of the public identifier on the <c>DOCTYPE</c> declaration.
        ///</summary>
        ///<value>
        /// The public identifier on the <c>DOCTYPE</c>. If there is no public identifier, <see langword="null"/>
        /// is returned.
        ///</value>
        string PublicId { get; }

        ///<summary>
        /// Gets the value of the system identifier on the <c>DOCTYPE</c> declaration.
        ///</summary>
        ///<value>
        /// The system identifier on the <c>DOCTYPE</c>. If there is no system identifier, <see langword="null"/>
        /// is returned.
        ///</value>
        string SystemId { get; }

        ///<summary>
        /// Gets the value of the document type definition (DTD) internal subset on the <c>DOCTYPE</c> declaration.
        ///</summary>
        ///<value>
        /// The DTD internal subset on the <c>DOCTYPE</c>. If there is no DTD internal subset, <see cref="String.Empty"/>
        /// is returned.
        ///</value>
        string InternalSubset { get; }
    }
}
