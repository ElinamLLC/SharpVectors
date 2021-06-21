using System;
using System.Net;
using System.IO;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;

namespace SharpVectors.Net
{
    /// <summary>
    /// Provides a response from a Uniform Resource Identifier (URI).
    /// </summary>
    /// <remarks>According to http://www.ietf.org/rfc/rfc2397.txt</remarks>
    [Serializable]
    public sealed class DataWebResponse : WebResponse
    {
        private static readonly Regex _reData          = new Regex(@"^data:(?<mediatype>.*?),(?<data>.*)$", RegexOptions.Singleline);
        private static readonly Regex _reSpaceRemover  = new Regex(@"\s", RegexOptions.Singleline);
        private static readonly Regex _reCharsetFinder = new Regex(@"charset=(?<charset>[^;]+)", RegexOptions.Singleline);

        private Encoding _contentEncoding = Encoding.ASCII;
        private string _contentType;
        private Uri _responseUri;

        private byte[] _decodedData;

        /// <summary>
        /// Initialize a new instance of the <see cref="DataWebResponse"/> with the specified URI.
        /// </summary>
        /// <param name="uri"></param>
        internal DataWebResponse(Uri uri)
        {
            this._responseUri = uri;

            string fullUri = HttpUtility.UrlDecode(uri.AbsoluteUri);
            fullUri = fullUri.Replace(' ', '+');

            // remove all whitespace
            fullUri = _contentType = _reSpaceRemover.Replace(fullUri, string.Empty);

            Match match = _reData.Match(fullUri);

            if (match.Success)
            {
                _contentType = match.Groups["mediatype"].Value;

                string data = match.Groups["data"].Value;

                if (_contentType.Length == 0)
                {
                    _contentType = "text/plain;charset=US-ASCII";
                }
                else if (_contentType.StartsWith(";", StringComparison.OrdinalIgnoreCase))
                {
                    if (_contentType.IndexOf(";charset=", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        _contentType = "text/plain" + _contentType;
                    }
                    else
                    {
                        throw new Exception("Malformed data URI");
                    }
                }

                if (_contentType.EndsWith(";base64", StringComparison.OrdinalIgnoreCase))
                {
                    _contentType = _contentType.Remove(_contentType.Length - 7, 7);
                    _decodedData = Convert.FromBase64String(data);
                }
                else
                {
                    Match charsetMatch = _reCharsetFinder.Match(_contentType);
                    if (charsetMatch.Success && charsetMatch.Groups["charset"].Success)
                    {
                        try
                        {
                            _contentEncoding = Encoding.GetEncoding(charsetMatch.Groups["charset"].Value);
                        }
                        catch (NotSupportedException)
                        {
                            _contentEncoding = Encoding.ASCII;
                        }
                    }

                    _decodedData = HttpUtility.UrlDecodeToBytes(data);
                }
            }
            else
            {
                throw new Exception("Malformed data URI");
            }
        }

        /// <summary>
        /// Gets the content length of data being received.
        /// </summary>
        /// <value>The number of bytes returned from the Internet resource.</value>
        public override long ContentLength
        {
            get {
                return _decodedData.Length;
            }
        }

        /// <summary>
        /// Gets the content encoding of data being received.
        /// </summary>
        /// <value>A <see cref="Encoding"/> specifying the encoding. The default is <see cref="Encoding.ASCII"/>.</value>
        public Encoding ContentEncoding
        {
            get {
                return _contentEncoding;
            }
        }

        /// <summary>
        /// Gets the content type of the data being received.
        /// </summary>
        /// <value>A string that contains the content type of the response.</value>
        public override string ContentType
        {
            get {
                return _contentType;
            }
        }

        /// <summary>
        /// Gets the URI of the Internet resource that actually responded to the request.
        /// </summary>
        /// <value>An instance of the <see cref="Uri"/> class that contains the URI of the Internet resource 
        /// that actually responded to the request.</value>
        public override Uri ResponseUri
        {
            get {
                return _responseUri;
            }
        }

        /// <summary>
        /// This returns the data stream from the Internet resource.
        /// </summary>
        /// <returns>An instance of the <see cref="Stream"/> class for reading data from the Internet resource.</returns>
        public override Stream GetResponseStream()
        {
            return new MemoryStream(_decodedData, false)
            {
                Position = 0
            };
        }
    }
}