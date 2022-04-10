using System;
using LazyCache;
using Microsoft.Extensions.Caching.Memory;

namespace Lexplorer.Helpers
{
	public static class LAppCacheExtensions
	{
		public static async Task<T> GetOrAddAsyncNonNull<T>(this IAppCache cache, string key, Func<Task<T>> addItemFactory, DateTimeOffset expires)
		{
			if (cache == null)
			{
				throw new ArgumentNullException("cache");
			}
			T retValue = await cache.GetOrAddAsync(key, addItemFactory, new MemoryCacheEntryOptions
			{
				AbsoluteExpiration = expires
			});
			if (retValue == null)
				cache.Remove(key);
			return retValue;
		}

		public static async Task<T> GetOrAddAsyncNonNull<T>(this IAppCache cache, string key, Func<Task<T>> addItemFactory)
		{
			if (cache == null)
			{
				throw new ArgumentNullException("cache");
			}
			T retValue = await cache.GetOrAddAsync(key, addItemFactory);
			if (retValue == null)
				cache.Remove(key);
			return retValue;
		}
	}
}
