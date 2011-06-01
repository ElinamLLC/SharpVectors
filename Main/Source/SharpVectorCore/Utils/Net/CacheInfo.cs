using System;

namespace SharpVectors.Net
{
    public sealed class CacheInfo
	{
        private DateTime expires;
        private Uri cachedUri;
        private DateTime lastModified;
        private string etag;
        private string contentType;

		public CacheInfo(DateTime expires, string etag, 
            DateTime lastModified, Uri cachedUri, string contentType)
		{
			this.expires = expires;
			this.etag = etag;
			this.lastModified = lastModified;
			this.cachedUri = cachedUri;
			this.contentType = contentType;
		}

		public DateTime Expires
		{
			get{return expires;}
		}

		public Uri CachedUri
		{
			get{return cachedUri;}
		}

		public DateTime LastModified
		{
			get{return lastModified;}
		}

		public string ETag
		{
			get{return etag;}
		}

		public string ContentType
		{
			get{return contentType;}
		}
	}
}
