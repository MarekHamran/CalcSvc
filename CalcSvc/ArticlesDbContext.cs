using CalcSvc.Model;
using Microsoft.EntityFrameworkCore;

namespace CalcSvc
{
	public class ArticlesDbContext : DbContext
	{
		public ArticlesDbContext() { }

		public ArticlesDbContext(DbContextOptions<ArticlesDbContext> options) : base(options) { }

		public DbSet<Site> Sites { get; set; }
		public DbSet<Author> Author { get; set; }
		public DbSet<Article> Articles { get; set; }
		public DbSet<Image> Images { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
			optionsBuilder.UseNpgsql("Host=postgres;Port=5432;Database=Articles;Username=postgres;Password=pgpassword");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Site>().HasKey(x => x.Id);
			modelBuilder.Entity<Site>().Property(x => x.Id).UseIdentityAlwaysColumn();

			modelBuilder.Entity<Author>().HasKey(x => x.Id);
			modelBuilder.Entity<Author>().Property(x => x.Id).UseIdentityAlwaysColumn();
			modelBuilder.Entity<Author>().HasIndex(x => x.Name).IsUnique();

			modelBuilder.Entity<Article>().HasKey(x => x.Id);
			modelBuilder.Entity<Article>().HasIndex(x => x.Title);
			modelBuilder.Entity<Article>().Property(x => x.Id).UseIdentityAlwaysColumn();

			modelBuilder.Entity<Image>().HasKey(x => x.Id);
			modelBuilder.Entity<Image>().Property(x => x.Id).UseIdentityAlwaysColumn();

			modelBuilder.Entity<Article>().HasMany<Author>(x => x.Author).WithMany();
			modelBuilder.Entity<Article>().HasOne<Site>(x => x.Site);

			modelBuilder.Entity<Author>().HasOne(x => x.Image);
		}
	}
}
