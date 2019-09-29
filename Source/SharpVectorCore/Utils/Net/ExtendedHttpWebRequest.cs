using System;
using System.Net;
using System.IO;
using System.IO.Compression;

namespace SharpVectors.Net
{
    public sealed class ExtendedHttpWebRequestCreator : IWebRequestCreate
    {
        public ExtendedHttpWebRequestCreator()
        {
        }

        WebRequest IWebRequestCreate.Create(Uri uri)
        {
            return new ExtendedHttpWebRequest(uri);
        }

        public ExtendedHttpWebRequest Create(Uri uri)
        {
            return new ExtendedHttpWebRequest(uri);
        }
    }

    [Serializable]
    public sealed class ExtendedHttpWebRequest : WebRequest
    {
        #region Private Fields

        private static ICacheManager _cacheManager;
        private Uri _requestUri;

        #endregion

        #region Constructors and Destructor

        public ExtendedHttpWebRequest(Uri uri)
        {
            _requestUri = uri;
        }

        #endregion

        #region Public Properties

        public override Uri RequestUri
        {
            get {
                return _requestUri;
            }
        }

        public static ICacheManager CacheManager
        {
            get {
                if (_cacheManager == null)
                {
                    _cacheManager = new NoCacheManager();
                }
                return _cacheManager;
            }
            set {
                _cacheManager = value;
            }
        }

        #endregion

        #region Public Methods

        public static bool Register()
        {
            if (!WebRequest.RegisterPrefix("http://", new ExtendedHttpWebRequestCreator()))
            {
                return false;
            }
            if (!WebRequest.RegisterPrefix("https://", new ExtendedHttpWebRequestCreator()))
            {
                return false;
            }

            return true;
        }

        public override WebResponse GetResponse()
        {
            CacheInfo cacheInfo = CacheManager.GetCacheInfo(RequestUri);

            WebRequest request = GetRequest(cacheInfo);
            WebResponse response = GetResponse(request, cacheInfo);

            if (response == null)
            {
                return null;
            }

            Stream stream = ProcessResponseStream(response);

            if (response is HttpWebResponse)
            {
                CacheInfo respCacheInfo = ProcessResponse(response);

                CacheManager.SetCacheInfo(RequestUri, respCacheInfo, stream);
            }

            return new ExtendedHttpWebResponse(RequestUri, response, stream, cacheInfo);
        }

        #endregion

        #region Private Methods

        private WebRequest GetRequest(CacheInfo cacheInfo)
        {
            WebRequest request;
            if (cacheInfo != null &&
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
            if (hRequest != null && cacheInfo != null && cacheInfo.CachedUri != null)
            {
                if (cacheInfo.ETag != null)
                {
                    hRequest.Headers["If-None-Match"] = cacheInfo.ETag;
                }
                if (cacheInfo.LastModified != DateTime.MinValue)
                {
                    hRequest.IfModifiedSince = cacheInfo.LastModified;
                }

                hRequest.Headers["Accept-Encoding"] = "deflate, gzip";
            }

            return request;
        }

        private WebResponse GetResponse(WebRequest request, CacheInfo cacheInfo)
        {
            WebResponse response = null;
            try
            {
                response = request.GetResponse();
            }
            catch (WebException webEx)
            {
                HttpWebResponse hresp2 = webEx.Response as HttpWebResponse;
                if (hresp2 != null)
                {
                    if (hresp2.StatusCode == HttpStatusCode.NotModified)
                    {

                        if (cacheInfo != null && cacheInfo.CachedUri != null)
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

        private CacheInfo ProcessResponse(WebResponse response)
        {
            HttpWebResponse hResponse = response as HttpWebResponse;
            CacheInfo cacheInfo = null;

            if (hResponse != null)
            {
                DateTime expires;
                if (hResponse.Headers["Expires"] != null)
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

        private Stream ProcessResponseStream(WebResponse response)
        {
            Stream respStream = response.GetResponseStream();

            string contentEnc = response.Headers["Content-Encoding"];
            if (contentEnc != null)
            {
                contentEnc = contentEnc.ToLower();
                if (string.Equals(contentEnc, "gzip", StringComparison.OrdinalIgnoreCase))
                {
                    respStream = new GZipStream(respStream, CompressionMode.Decompress);
                }
                else if (string.Equals(contentEnc, "deflate", StringComparison.OrdinalIgnoreCase))
                {
                    respStream = new DeflateStream(respStream, CompressionMode.Decompress);
                }
            }
            else if (_requestUri.ToString().EndsWith(".svgz", StringComparison.OrdinalIgnoreCase))
            {
                // TODO: this is an ugly hack for .svgz files. Fix later!
                respStream = new GZipStream(respStream, CompressionMode.Decompress);
            }

            Stream stream = new MemoryStream();
            int count = 0;
            byte[] buffer = new byte[4096];
            while ((count = respStream.Read(buffer, 0, 4096)) > 0)
            {
                stream.Write(buffer, 0, count);
            }

            stream.Position = 0;

            respStream.Dispose();

            return stream;
        }

        #endregion
    }
}
