using System;
using System.IO;
using System.Net;

namespace SharpVectors.Net
{
    [Serializable]
    public sealed class ExtendedHttpWebResponse : WebResponse
    {
        private Uri _responseUri;
        private Stream _responseStream;
        private CacheInfo _cacheInfo;
        private WebResponse _response;

        public ExtendedHttpWebResponse(Uri responseUri, WebResponse response, Stream responseStream, CacheInfo cacheInfo)
        {
            _responseUri    = responseUri;
            _response       = response;
            _responseStream = responseStream;
            _cacheInfo      = cacheInfo;
        }

        public override Stream GetResponseStream()
        {
            if (_responseStream != null && _responseStream.CanSeek)
            {
                _responseStream.Position = 0;
                return _responseStream;
            }
            return _response.GetResponseStream();
        }

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
