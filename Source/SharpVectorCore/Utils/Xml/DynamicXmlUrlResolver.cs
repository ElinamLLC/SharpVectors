using System;
using System.IO;
using System.Xml;

namespace SharpVectors.Xml
{
    /// <summary>
    /// Provides event-based interface to resolve external XML resources named by a Uniform Resource Identifier (URI).
    /// </summary>
    public sealed class DynamicXmlUrlResolver : XmlUrlResolver
    {
        /// <summary>
        /// Event handler type
        /// </summary>
        public delegate string ResolveEventHandler(string relativeUri);

        /// <summary>
        /// Fires when GetEntity is called
        /// </summary>
        public delegate object GettingEntityEventHandler(Uri absoluteUri, string role, Type ofObjectToReturn);

        /// <summary>
        /// Occurs when trying to resolve an Uri.
        /// </summary>
        public event ResolveEventHandler Resolving;

        /// <summary>
        /// Occurs when getting entity.
        /// </summary>
        public event GettingEntityEventHandler GettingEntity;

        public static readonly UrlResolvePolicy UrlDefaultPolicy = new UrlResolvePolicy(DtdProcessing.Parse);

        private static UrlResolvePolicy _urlPolicy;

        public DynamicXmlUrlResolver() 
        {
        }

        public static UrlResolvePolicy UrlPolicy
        {
            get {
                if (_urlPolicy == null)
                {
                    _urlPolicy = UrlDefaultPolicy;
                }
                return _urlPolicy;
            }
            set {
                _urlPolicy = value;
            }
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
            if (this.GettingEntity != null)
            {
                object entity = GettingEntity(absoluteUri, role, ofObjectToReturn);
                if (entity != null)
                    return entity;
            }

            UrlResolvePolicy urlPolicy = DynamicXmlUrlResolver.UrlPolicy;
            if (urlPolicy.Processing == DtdProcessing.Parse)
            {
                if (urlPolicy.Entity.HasFlag(UrlResolveTypes.Resource))
                {
                    return Stream.Null;
                }
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
            if (this.Resolving != null)
                relativeUri = Resolving(relativeUri);

            return base.ResolveUri(baseUri, relativeUri);
        }
    }
}
