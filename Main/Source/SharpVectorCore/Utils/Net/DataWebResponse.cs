using System;
using System.Net;
using System.IO;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;

namespace SharpVectors.Net
{
	/// <summary>
	/// Summary description for DataWebResponse.
	/// </summary>
	/// <remarks>According to http://www.ietf.org/rfc/rfc2397.txt</remarks>
	public class DataWebResponse : WebResponse
	{
		private byte[] decodedData;

		private static Regex re = new Regex(@"^data:(?<mediatype>.*?),(?<data>.*)$", RegexOptions.Singleline);
		private static Regex wsRemover = new Regex(@"\s", RegexOptions.Singleline);
		private static Regex charsetFinder = new Regex(@"charset=(?<charset>[^;]+)", RegexOptions.Singleline);

		internal DataWebResponse(Uri uri)
		{
			this.responseUri = uri;
			
			string fullUri = HttpUtility.UrlDecode(uri.AbsoluteUri);
			fullUri = fullUri.Replace(' ', '+');

			// remove all whitespace
			fullUri = contentType = wsRemover.Replace(fullUri, "");
			
			Match match = re.Match(fullUri);

			if(match.Success)
			{
				contentType = match.Groups["mediatype"].Value;

				string data = match.Groups["data"].Value;
			

				if(contentType.Length == 0)
				{
					contentType = "text/plain;charset=US-ASCII";
				}
				else if (contentType.StartsWith(";"))
				{
					if (contentType.IndexOf(";charset=") > 0)
					{
						contentType = "text/plain" + contentType;
					}
					else
					{
						throw new Exception("Malformed data URI");
					}
				}

				if(contentType.EndsWith(";base64"))
				{
					contentType = contentType.Remove(contentType.Length - 7, 7);
					decodedData = Convert.FromBase64String(data);
				}
				else
				{
					Match charsetMatch = charsetFinder.Match(contentType);
					if(charsetMatch.Success && charsetMatch.Groups["charset"].Success)
					{
						try
						{
							contentEncoding = Encoding.GetEncoding(charsetMatch.Groups["charset"].Value);
						}
						catch(NotSupportedException)
						{
							contentEncoding = Encoding.ASCII;
						}
					}
					
					decodedData = HttpUtility.UrlDecodeToBytes(data);
				}
			}
			else
			{
				throw new Exception("Malformed data URI");
			}
		}

		public override long ContentLength
		{
			get
			{
				return decodedData.Length;
			}
		}

		private Encoding contentEncoding = Encoding.ASCII;
		public Encoding ContentEncoding
		{
			get
			{
				return contentEncoding;
			}
		}

		private string contentType;
		public override string ContentType
		{
			get
			{
				return contentType;
			}
		}

		private Uri responseUri;
		public override Uri ResponseUri
		{
			get
			{
				return responseUri;
			}
		}

		public override Stream GetResponseStream()
		{
			MemoryStream ms; 
			ms = new MemoryStream(decodedData, false);
			ms.Position = 0;
			return ms;
		}
	}
}