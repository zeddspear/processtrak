using System.Reflection;
using Microsoft.EntityFrameworkCore;
using processtrak_backend.Models;

namespace processtrak_backend.Api.data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Automatically configure UUID generation for GUID primary keys
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
            }

            // Automatically set CreatedAt, UpdatedAt, and DeletedAt fields
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var clrType = entityType.ClrType;

                if (typeof(BaseEntity).IsAssignableFrom(clrType))
                {
                    modelBuilder
                        .Entity(clrType)
                        .Property<DateTime>("createdAt")
                        .HasDefaultValueSql("now() at time zone 'utc'")
                        .ValueGeneratedOnAdd();

                    modelBuilder
                        .Entity(clrType)
                        .Property<DateTime?>("updatedAt")
                        .IsRequired(false)
                        .ValueGeneratedOnUpdate();

                    modelBuilder.Entity(clrType).Property<DateTime?>("deletedAt").IsRequired(false);
                }
            }

            // Apply configurations from all IEntityTypeConfiguration<> in the assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

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

        // Define DbSet properties for your entities
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<OtpCode> OtpCodes { get; set; } = null!;
    }
}
