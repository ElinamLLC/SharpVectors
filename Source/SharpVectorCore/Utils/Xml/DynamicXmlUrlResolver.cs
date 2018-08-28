using System;
using System.Xml;

namespace SharpVectors.Xml
{
    /// <summary>
    /// Hook URL solving
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

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            if (this.GettingEntity != null)
            {
                object entity = GettingEntity(absoluteUri, role, ofObjectToReturn);
                if (entity != null)
                    return entity;
            }

            return base.GetEntity(absoluteUri, role, ofObjectToReturn);
        }

        /// <summary>
        /// Resolves the absolute URI from the base and relative URIs.
        /// </summary>
        /// <param name="baseUri">The base URI used to resolve the relative URI.</param>
        /// <param name="relativeUri">The URI to resolve. The URI can be absolute or relative. If absolute, this value effectively replaces the <paramref name="baseUri"/> value. If relative, it combines with the <paramref name="baseUri"/> to make an absolute URI.</param>
        /// <returns>
        /// A <see cref="T:System.Uri"/> representing the absolute URI, or null if the relative URI cannot be resolved.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="baseUri "/>is null or <paramref name="relativeUri"/> is null</exception>
        public override Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            if (this.Resolving != null)
                relativeUri = Resolving(relativeUri);

            return base.ResolveUri(baseUri, relativeUri);
        }
    }
}
