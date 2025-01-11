using Microsoft.EntityFrameworkCore;
using processtrak_backend.Models;

namespace processtrak_backend.Api.data
{
    public class AppDbContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public AppDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseNpgsql(Configuration.GetConnectionString("AppDatabaseURI"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var primaryKey = entityType.FindPrimaryKey();
                if (primaryKey != null && primaryKey.Properties.Count == 1)
                {
                    var property = primaryKey.Properties[0];
                    if (property.ClrType == typeof(Guid))
                    {
                        property.SetDefaultValueSql("uuid_generate_v4()");
                    }
                }

                // Automatically set CreatedAt, UpdatedAt, and DeletedAt
                modelBuilder
                    .Entity(entityType.ClrType)
                    .Property("createdAt")
                    .HasDefaultValueSql("now() at time zone 'utc'");
                modelBuilder.Entity(entityType.ClrType).Property("updatedAt").IsRequired(false);
                modelBuilder.Entity(entityType.ClrType).Property("deletedAt").IsRequired(false);
            }
            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.createdAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.updatedAt = DateTime.UtcNow;
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        public required DbSet<User> Users { get; set; }
        public required DbSet<OtpCode> OtpCodes { get; set; }
    }
}
