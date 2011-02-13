using System;
using System.Diagnostics;
using System.Net;
using System.Xml;
using System.IO;
using System.IO.Compression;

namespace SharpVectors.Net
{
	public class ExtendedHttpWebRequestCreator : IWebRequestCreate
	{
		public ExtendedHttpWebRequestCreator(){}
		public WebRequest Create(Uri uri){return new ExtendedHttpWebRequest(uri);}
	}

	public class ExtendedHttpWebRequest : WebRequest
	{
		#region CacheManager
		private static ICacheManager cacheManager = new NoCacheManager();
		public static ICacheManager CacheManager
		{
			get{return cacheManager;}
			set{cacheManager = value;}
		}
		#endregion

		#region Registration
		public static bool Register()
		{
			return WebRequest.RegisterPrefix("http://", new ExtendedHttpWebRequestCreator());
		}
		#endregion

		#region Constructors
		public ExtendedHttpWebRequest(Uri uri)
		{
			requestUri = uri;
		}
		#endregion

		#region RequestUri
		private Uri requestUri;
		public override Uri RequestUri
		{
			get
			{
				return requestUri;
			}
		}
		#endregion

		private WebRequest getRequest(CacheInfo cacheInfo)
		{
			WebRequest request;
			if(cacheInfo != null && 
				cacheInfo.CachedUri != null && 
				cacheInfo.Expires > DateTime.Now)
			{
				request = WebRequest.Create(cacheInfo.CachedUri);
			}
			else
			{
				request = WebRequest.CreateDefault(RequestUri);
			}

			HttpWebRequest hRequest = request as HttpWebRequest;
			if(hRequest != null && cacheInfo != null && cacheInfo.CachedUri != null)
			{
				if(cacheInfo.ETag != null)
				{
					hRequest.Headers["If-None-Match"] = cacheInfo.ETag;
				}
				if(cacheInfo.LastModified != DateTime.MinValue)
				{
					hRequest.IfModifiedSince = cacheInfo.LastModified;
				}

				hRequest.Headers["Accept-Encoding"] = "deflate, gzip" ;
			}

			return request;
		}

		private WebResponse getResponse(WebRequest request, CacheInfo cacheInfo)
		{
			WebResponse response = null;
			try
			{
				response = request.GetResponse();
			}
			catch(WebException webEx)
			{
				HttpWebResponse hresp2 = webEx.Response as HttpWebResponse;
				if(hresp2 != null)
				{
					if(hresp2.StatusCode == HttpStatusCode.NotModified)
					{
						
						if(cacheInfo != null && cacheInfo.CachedUri != null)
						{
							request = WebRequest.Create(cacheInfo.CachedUri);
						}
						else
						{
							request = WebRequest.Create(RequestUri);
						}
						response = request.GetResponse();
					}
				}
			}

			return response;
		}

		private CacheInfo processResponse(WebResponse response)
		{
			HttpWebResponse hResponse = response as HttpWebResponse;
			CacheInfo cacheInfo = null;

			if(hResponse != null)
			{
				DateTime expires;
				if(hResponse.Headers["Expires"] != null)
				{
					expires = DateTime.Parse(hResponse.Headers["Expires"]);
				}
				else
				{
					expires = DateTime.MinValue;
				}
			
				cacheInfo = new CacheInfo(expires, hResponse.Headers["Etag"], hResponse.LastModified, null, hResponse.ContentType);
			}

			return cacheInfo;
		}

		private Stream processResponseStream(WebResponse response)
		{
			Stream respStream = response.GetResponseStream();

			string contentEnc = response.Headers["Content-Encoding"];
			if ( contentEnc != null) 
			{
				contentEnc = contentEnc.ToLower() ;
				if ( contentEnc == "gzip" ) 
				{
					respStream = new GZipStream(respStream, CompressionMode.Decompress) ;
				}
				else if ( contentEnc == "deflate" ) 
				{
                    respStream = new DeflateStream(respStream, CompressionMode.Decompress);
				}
			}
			else if(requestUri.ToString().EndsWith(".svgz"))
			{
				// TODO: this is an ugly hack for .svgz files. Fix later!
                respStream = new GZipStream(respStream, CompressionMode.Decompress);
			}
				
			Stream stream = new MemoryStream();
			int count = 0;
			byte[] buffer = new byte[4096];
			while((count = respStream.Read(buffer, 0, 4096)) > 0) stream.Write(buffer, 0, count);

			stream.Position = 0;
			
			((IDisposable)respStream).Dispose();

			return stream;
		}

		public override WebResponse GetResponse()
		{
			CacheInfo cacheInfo = CacheManager.GetCacheInfo(RequestUri);

			WebRequest request = getRequest(cacheInfo);
			WebResponse response = getResponse(request, cacheInfo);

			Stream stream = processResponseStream(response);

			if(response is HttpWebResponse)
			{
				CacheInfo respCacheInfo = processResponse(response);

				CacheManager.SetCacheInfo(RequestUri, respCacheInfo, stream);
			}

			return new ExtendedHttpWebResponse(RequestUri, response, stream, cacheInfo);
		}
	}
}
