using System;

namespace SharpVectors.Net
{
	public class CacheInfo
	{
		public CacheInfo(DateTime expires, string etag, DateTime lastModified, Uri cachedUri, string contentType)
		{
			this.expires = expires;
			this.etag = etag;
			this.lastModified = lastModified;
			this.cachedUri = cachedUri;
			this.contentType = contentType;
		}

		private DateTime expires;
		public DateTime Expires
		{
			get{return expires;}
		}

		private Uri cachedUri;
		public Uri CachedUri
		{
			get{return cachedUri;}
		}

		private DateTime lastModified;
		public DateTime LastModified
		{
			get{return lastModified;}
		}

		private string etag;
		public string ETag
		{
			get{return etag;}
		}

		private string contentType;
		public string ContentType
		{
			get{return contentType;}
		}
	}
}
