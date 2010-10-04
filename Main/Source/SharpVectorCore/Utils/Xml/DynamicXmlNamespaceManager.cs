using System.Xml;

namespace SharpVectors.Xml
{
    /// <summary>
    /// XML namespace manager allowing to fire events when namespace is not found
    /// </summary>
    public sealed class DynamicXmlNamespaceManager : XmlNamespaceManager
    {
        /// <summary>
        /// Event handler type
        /// </summary>
        public delegate string ResolveEventHandler(string prefix);

        /// <summary>
        /// Occurs when trying to resolve an unknown namespace.
        /// </summary>
        public event ResolveEventHandler Resolve;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicXmlNamespaceManager"/> class.
        /// </summary>
        public DynamicXmlNamespaceManager()
            : base(new NameTable())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicXmlNamespaceManager"/> class.
        /// </summary>
        /// <param name="xmlNameTable">The XML name table.</param>
        public DynamicXmlNamespaceManager(XmlNameTable xmlNameTable)
            : base(xmlNameTable)
        {
        }

        /// <summary>
        /// Gets the namespace URI for the specified prefix.
        /// </summary>
        /// <param name="prefix">The prefix whose namespace URI you want to resolve. To match the default namespace, pass String.Empty.</param>
        /// <returns>
        /// Returns the namespace URI for <paramref name="prefix"/> or null if there is no mapped namespace. The returned string is atomized.
        /// For more information on atomized strings, see <see cref="T:System.Xml.XmlNameTable"/>.
        /// </returns>
        public override string LookupNamespace(string prefix)
        {
            string uri = base.LookupNamespace(prefix);
            if (uri == null)
                uri = this.Resolve(prefix);

            return uri;
        }
    }
}
