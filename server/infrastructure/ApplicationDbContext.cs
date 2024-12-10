// Infrastructure/Data/ApplicationDbContext.cs

using core;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<MyDomainModel> YourEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Configure Primary key and exit
            modelBuilder.Entity<MyDomainModel>().HasKey(e => e.Id);
            base.OnModelCreating(modelBuilder);
        }
    }
}
