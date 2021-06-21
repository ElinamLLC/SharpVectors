using System;

namespace SharpVectors.Net
{
    /// <summary>
    /// A class encapsulating the caching information of the HTTP responses.
    /// </summary>
    [Serializable]
    public sealed class CacheInfo
    {
        private DateTime _expires;
        private Uri _cachedUri;
        private DateTime _lastModified;
        private string _etag;
        private string _contentType;

        /// <summary>
        /// Initializes an instance of the <see cref="CacheInfo"/> class with the specified parameters.
        /// </summary>
        /// <param name="expires">This specifies the date and time after which the accompanying body data should be considered stale.</param>
        /// <param name="etag">A value which specifies the current value for the requested variant.</param>
        /// <param name="lastModified">A value which specifies the date and time at which the accompanying body data was last modified.</param>
        /// <param name="cachedUri">A value specifying the cached URI.</param>
        /// <param name="contentType">A value which specifies the MIME type of the accompanying body data.</param>
        public CacheInfo(DateTime expires, string etag, DateTime lastModified, Uri cachedUri, string contentType)
        {
            _expires      = expires;
            _etag         = etag;
            _lastModified = lastModified;
            _cachedUri    = cachedUri;
            _contentType  = contentType;
        }

        /// <summary>
        /// Gets a value which specifies the date and time after which the accompanying body data should be considered stale.
        /// </summary>
        /// <value>A <see cref="DateTime"/> specifying the expiring date and time.</value>
        public DateTime Expires
        {
            get { return _expires; }
        }

        /// <summary>
        /// Gets a value specifying the cached URI.
        /// </summary>
        /// <value>A <see cref="Uri"/> specifying the cached URI.</value>
        public Uri CachedUri
        {
            get { return _cachedUri; }
        }

        /// <summary>
        /// Gets a value which specifies the date and time at which the accompanying body data was last modified.
        /// </summary>
        /// <value>A <see cref="DateTime"/> specifying the date and time at which the accompanying body data was last modified.</value>
        public DateTime LastModified
        {
            get { return _lastModified; }
        }

        /// <summary>
        /// Gets a value which specifies the current value for the requested variant.
        /// </summary>
        /// <value>A string specifying the current value for the requested variant.</value>
        public string ETag
        {
            get { return _etag; }
        }

        /// <summary>
        /// Gets a value which specifies the MIME type of the accompanying body data.
        /// </summary>
        /// <value>A string specifying the MIME type of the accompanying body data.</value>
        public string ContentType
        {
            get { return _contentType; }
        }
    }
}
