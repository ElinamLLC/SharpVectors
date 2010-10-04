using System;
using System.Net;

namespace SharpVectors.Net
{
	public class DataWebRequest : WebRequest, IWebRequestCreate
	{
		public static bool Register()
		{
      return WebRequest.RegisterPrefix("data", new DataWebRequest());
		}

		public new WebRequest Create(Uri uri)
		{
			return new DataWebRequest(uri);
		}

		//only for use from Register();
		private DataWebRequest()
		{
		}

		public DataWebRequest(Uri uri)
		{
			requestUri = uri;
		}

		private Uri requestUri;
		public override Uri RequestUri
		{
			get
			{
				return requestUri;
			}
		}

		public override WebResponse GetResponse()
		{
			return new DataWebResponse(RequestUri);
		}
	}
}
