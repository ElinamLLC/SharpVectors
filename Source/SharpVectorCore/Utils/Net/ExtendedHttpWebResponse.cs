using System;
using System.IO;
using System.Net;

namespace SharpVectors.Net
{
    /// <summary>
    /// A wrapper of the <see cref="WebResponse"/> class, which provides a response from a 
    /// Uniform Resource Identifier (URI), with support for a custom reponse caching.
    /// </summary>
    [Serializable]
    public sealed class ExtendedHttpWebResponse : WebResponse
    {
        private Uri _responseUri;
        private Stream _responseStream;
        private CacheInfo _cacheInfo;
        private WebResponse _response;

        /// <summary>
        /// Initializes an instance of the <see cref="ExtendedHttpWebResponse"/> class with the specified parameters.
        /// </summary>
        /// <param name="responseUri">The URI of the Internet resource that actually responded to the request.</param>
        /// <param name="response">The wrapped <see cref="WebResponse"/> instance, that provides a response from a Uniform Resource Identifier (URI). </param>
        /// <param name="responseStream">The data stream from the Internet resource.</param>
        /// <param name="cacheInfo">The caching information.</param>
        public ExtendedHttpWebResponse(Uri responseUri, WebResponse response, Stream responseStream, CacheInfo cacheInfo)
        {
            _responseUri    = responseUri;
            _response       = response;
            _responseStream = responseStream;
            _cacheInfo      = cacheInfo;
        }

        /// <summary>
        /// Gets the data stream from the Internet resource.
        /// </summary>
        /// <returns>An instance of the <see cref="Stream"/> class for reading data from the Internet resource.</returns>
        public override Stream GetResponseStream()
        {
            if (_responseStream != null && _responseStream.CanSeek)
            {
                _responseStream.Position = 0;
                return _responseStream;
            }
            return _response.GetResponseStream();
        }

        /// <summary>
        /// Gets the content type of the data being received.
        /// </summary>
        /// <value>A string that contains the content type of the response.</value>
        public override string ContentType
        {
            get {
                if (!(_response is HttpWebResponse) && _cacheInfo != null)
                {
                    return _cacheInfo.ContentType;
                }
                return _response.ContentType;
            }
        }

        /// <summary>
        /// Gets the URI of the Internet resource that actually responded to the request.
        /// </summary>
        /// <value>An instance of the <see cref="Uri"/> class that contains the URI of the Internet resource that 
        /// actually responded to the request.</value>
        public override Uri ResponseUri
        {
            get {
                if (!(_response is HttpWebResponse) && _cacheInfo != null)
                {
                    return _cacheInfo.CachedUri;
                }
                return _response.ResponseUri;
            }
        }
    }
}
