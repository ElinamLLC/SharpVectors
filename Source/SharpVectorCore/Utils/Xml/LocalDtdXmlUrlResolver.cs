using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace SharpVectors.Xml
{
    /// <summary>
    /// Used to redirect external DTDs to local copies.
    /// </summary>
    public sealed class LocalDtdXmlUrlResolver : XmlUrlResolver
    {
        private Dictionary<string, string> knownDtds;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDtdXmlUrlResolver"/> class.
        /// </summary>
        public LocalDtdXmlUrlResolver()
            : base()
        {
            knownDtds = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// This adds local DTD file with the specified public identifier on the entity.
        /// </summary>
        /// <param name="publicIdentifier">The public identifier on the entity.</param>
        /// <param name="localFile">A full path to the local DTD file.</param>
        public void AddDtd(string publicIdentifier, string localFile)
        {
            knownDtds.Add(publicIdentifier, localFile);
        }

        /// <summary>
        /// Maps a URI to an object that contains the actual resource.
        /// </summary>
        /// <param name="absoluteUri">The URI returned from <see cref="ResolveUri(Uri, String)"/>.</param>
        /// <param name="role">Currently not used.</param>
        /// <param name="ofObjectToReturn">The type of object to return. The current implementation only returns <see cref="Stream"/> objects.</param>
        /// <returns>A stream object or <see langword="null"/> if a type other than stream is specified.</returns>
        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            if (absoluteUri != null && knownDtds.ContainsKey(absoluteUri.AbsoluteUri))
            {
                // ignore the known DTDs
                return Stream.Null;
            }

            if (absoluteUri == null)
            {
                // ignore null URIs
                return Stream.Null;
            }

            return base.GetEntity(absoluteUri, role, ofObjectToReturn);
        }

        /// <summary>
        /// Resolves the absolute URI from the base and relative URIs.
        /// </summary>
        /// <param name="baseUri">The base URI used to resolve the relative URI.</param>
        /// <param name="relativeUri">The URI to resolve. The URI can be absolute or relative. 
        /// If absolute, this value effectively replaces the <paramref name="baseUri"/> value. 
        /// If relative, it combines with the <paramref name="baseUri"/> to make an absolute URI.</param>
        /// <returns>
        /// A <see cref="T:System.Uri"/> representing the absolute URI, or <see langword="null"/> if the relative URI cannot be resolved.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="baseUri"/>is null or <paramref name="relativeUri"/> is null</exception>
        public override Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            if (relativeUri.StartsWith("#", StringComparison.OrdinalIgnoreCase))
                return null;
            if (relativeUri.IndexOf("-//", StringComparison.OrdinalIgnoreCase) > -1)
                return null;

            return base.ResolveUri(baseUri, relativeUri);
        }
    }
}
