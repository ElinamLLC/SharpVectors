using System;
using System.Net;

namespace SharpVectors.Net
{
    /// <summary>
    /// Makes a request to a Uniform Resource Identifier (URI) with support for "data" type scheme or prefix.
    /// </summary>
    [Serializable]
    public sealed class DataWebRequest : WebRequest, IWebRequestCreate
    {
        private Uri _requestUri;

        /// <summary>
        /// Initializes an instance of the <see cref="DataWebRequest"/> class.
        /// </summary>
        private DataWebRequest()
        {
            DataSecurityProtocols.Initialize();
        }

        /// <summary>
        /// Initializes an instance of the <see cref="DataWebRequest"/> class with the specified URI.
        /// </summary>
        /// <param name="uri"></param>
        public DataWebRequest(Uri uri)
        {
            _requestUri = uri;
        }

        /// <summary>
        /// This registers the "data" type scheme or URI prefix. 
        /// </summary>
        /// <returns>This returns <see lanword="true"/> if the registration is successful; otherwise, <see lanword="false"/>.</returns>
        public static bool Register()
        {
            return WebRequest.RegisterPrefix("data", new DataWebRequest());
        }

        /// <summary>
        /// Initializes a new <see cref="WebRequest"/> instance for the specified URI scheme.
        /// </summary>
        /// <param name="uri">A <see cref="Uri"/> containing the URI of the requested resource.</param>
        /// <returns>A <see cref="DataWebRequest"/> descendant for the specified URI scheme.</returns>
        public new WebRequest Create(Uri uri)
        {
            return new DataWebRequest(uri);
        }

        /// <summary>
        /// Gets the URI of the Internet resource associated with the request.
        /// </summary>
        /// <value>A <see cref="Uri"/> representing the resource associated with the request.</value>
        public override Uri RequestUri
        {
            get {
                return _requestUri;
            }
        }

        /// <summary>
        /// This returns a response to an Internet request.
        /// </summary>
        /// <returns>A <see cref="DataWebResponse"/> containing the response to the Internet request.</returns>
        public override WebResponse GetResponse()
        {
            return new DataWebResponse(RequestUri);
        }
    }
}
