using System;
using System.Xml;
using System.IO;
using System.Net;

namespace SharpVectors.Net
{
	public class NoCacheManager : ICacheManager
	{
		public NoCacheManager()
		{
		}

		public CacheInfo GetCacheInfo(Uri uri)
		{
			return null;
		}

		public void SetCacheInfo(Uri uri, CacheInfo cacheInfo, Stream stream)
		{
		}
	}
}
