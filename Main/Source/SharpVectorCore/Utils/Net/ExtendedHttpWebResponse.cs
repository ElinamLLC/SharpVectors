using System;
using System.Net;
using System.IO;

namespace SharpVectors.Net
{
	public class ExtendedHttpWebResponse : WebResponse
	{
		private WebResponse response;
		private Uri responseUri;
		private Stream responseStream;
		private CacheInfo cacheInfo;

		public ExtendedHttpWebResponse(Uri responseUri, WebResponse response, Stream responseStream, CacheInfo cacheInfo)
		{
			this.responseUri = responseUri;
			this.response = response;
			this.responseStream = responseStream;
			this.cacheInfo = cacheInfo;
		}

		public override Stream GetResponseStream()
		{
			if(responseStream != null && responseStream.CanSeek)
			{
				responseStream.Position = 0;
				return responseStream;
			}
			else
			{
				return response.GetResponseStream();
			}
		}

		public override string ContentType
		{
			get
			{
				if(!(response is HttpWebResponse) && cacheInfo != null)
				{
					return cacheInfo.ContentType;
				}
				else
				{
					return response.ContentType;
				}
			}
		}

		public override Uri ResponseUri
		{
			get
			{
				if(!(response is HttpWebResponse) && cacheInfo != null)
				{
					return cacheInfo.CachedUri;
				}
				else
				{
					return response.ResponseUri;
				}
			}
		}
	}
}
