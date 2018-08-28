using System;
using System.IO;

namespace SharpVectors.Net
{
	public sealed class NoCacheManager : ICacheManager
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
