using System;
using System.Net;

namespace SharpVectors.Net
{
    [Serializable]
    public sealed class DataWebRequest : WebRequest, IWebRequestCreate
    {
        private Uri _requestUri;

        //only for use from Register();
        private DataWebRequest()
        {
        }

        public DataWebRequest(Uri uri)
        {
            _requestUri = uri;
        }

        public static bool Register()
        {
            return WebRequest.RegisterPrefix("data", new DataWebRequest());
        }

        public new WebRequest Create(Uri uri)
        {
            return new DataWebRequest(uri);
        }

        public override Uri RequestUri
        {
            get {
                return _requestUri;
            }
        }

        public override WebResponse GetResponse()
        {
            return new DataWebResponse(RequestUri);
        }
    }
}
