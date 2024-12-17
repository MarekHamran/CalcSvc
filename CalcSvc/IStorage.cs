namespace CalcSvc
{
	public class StorageEntry
	{
		private StorageEntry() { }

		public StorageEntry(decimal @value)
		{
			Value = @value;
			Age = DateTime.UtcNow;
		}

		public DateTime Age { get; }
		public decimal Value { get; }
	}

	public interface IStorage
	{
		void Put(int key, StorageEntry entry);
		StorageEntry? Get(int key);
	}
}
