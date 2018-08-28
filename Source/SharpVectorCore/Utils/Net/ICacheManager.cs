using System;
using System.IO;

namespace SharpVectors.Net
{
	public interface ICacheManager
	{
		CacheInfo GetCacheInfo(Uri uri);
		void SetCacheInfo(Uri uri, CacheInfo cacheInfo, Stream stream);
	}
}
