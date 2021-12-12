using System;
using System.IO;

namespace SharpVectors.Net
{
	/// <summary>
	/// An implementation of the <see cref="ICacheManager"/> interface providing no caching to internet requests.
	/// </summary>
	public sealed class NoCacheManager : ICacheManager
	{
		/// <summary>
		/// Initializes an instance of the <see cref="NoCacheManager"/> class.
		/// </summary>
		public NoCacheManager()
		{
		}

		/// <summary>
		/// Get a caching information for the internet requests.
		/// </summary>
		/// <param name="uri">The uniform resource identifier (URI) of the Web resource.</param>
		/// <returns>This always return <see langword="null"/>, for no caching.</returns>
		public CacheInfo GetCacheInfo(Uri uri)
		{
			return null;
		}

		/// <summary>
		/// Sets the internet request cache information.
		/// </summary>
		/// <param name="uri">The uniform resource identifier (URI) of the Web resource.</param>
		/// <param name="cacheInfo">The caching information for the specified URI.</param>
		/// <param name="stream">The internet request contents to be cached.</param>
		/// <remarks>Since this provides no caching, any specified parameters are ignored.</remarks>
		public void SetCacheInfo(Uri uri, CacheInfo cacheInfo, Stream stream)
		{
		}
	}
}
