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

        public LocalDtdXmlUrlResolver()
            : base()
        {
            knownDtds = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public void AddDtd(string publicIdentifier, string localFile)
        {
            knownDtds.Add(publicIdentifier, localFile);
        }

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            if (absoluteUri != null && knownDtds.ContainsKey(absoluteUri.AbsoluteUri))
            {
                // ignore the known DTDs
                return Stream.Null;
            }
            else if (absoluteUri == null)
            {
                // ignore null URIs
                return Stream.Null;
            }
            else
            {
                return base.GetEntity(absoluteUri, role, ofObjectToReturn);
            }
        }

        public override Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            if (relativeUri.StartsWith("#"))
                return null;
            else if (relativeUri.IndexOf("-//") > -1)
                return null;
            else
                return base.ResolveUri(baseUri, relativeUri);
        }
    }
}
