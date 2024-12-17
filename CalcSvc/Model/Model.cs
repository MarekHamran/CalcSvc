namespace CalcSvc.Model
{
	public class Author
	{
		public long Id { get; set; } // Primary key
		public required string Name { get; set; } // Unique index
		public virtual Image? Image { get; set; } // One-To-One
	}
	public class Article
	{
		public long Id { get; set; } // Primary key
		public required string Title { get; set; } // Index
		public virtual ICollection<Author>? Author { get; set; } // Many-To-Many
		public required virtual Site Site { get; set; } // One-To-Many
	}
	public class Site
	{
		public long Id { get; set; } // Primary key
		public DateTimeOffset CreatedAt { get; set; }
	}
	public class Image
	{
		public long Id { get; set; } // Primary key
		public string? Description { get; set; }
	}
}
