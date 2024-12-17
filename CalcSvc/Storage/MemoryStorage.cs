using Microsoft.Extensions.Caching.Memory;

namespace CalcSvc.Storage
{
	public class MemoryStorage : IStorage
	{
		private MemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());

		public StorageEntry? Get(int key)
		{
			return _memoryCache.Get<StorageEntry>(key);
		}

		public void Put(int key, StorageEntry entry)
		{
			_memoryCache.Set(key, entry);
		}
	}
}
