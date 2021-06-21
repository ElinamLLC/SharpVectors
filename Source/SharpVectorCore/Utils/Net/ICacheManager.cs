using System;
using System.IO;

namespace SharpVectors.Net
{
	/// <summary>
	/// An interface that defines and manages the caching information for the extended <see cref="WebRequest"/> class
	/// used by this library.
	/// </summary>
	public interface ICacheManager
	{
		/// <summary>
		/// Gets the internet request cache information.
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		CacheInfo GetCacheInfo(Uri uri);

		/// <summary>
		/// Sets the internet request cache information.
		/// </summary>
		/// <param name="uri">The uniform resource identifier (URI) of the Web resource.</param>
		/// <param name="cacheInfo">The caching information for the specified URI.</param>
		/// <param name="stream">The internet request contents to be cached.</param>
		void SetCacheInfo(Uri uri, CacheInfo cacheInfo, Stream stream);
	}
}
