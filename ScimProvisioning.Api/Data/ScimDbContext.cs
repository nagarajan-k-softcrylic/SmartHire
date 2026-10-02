using Microsoft.EntityFrameworkCore;
using ScimProvisioning.Api.Entities;

namespace ScimProvisioning.Api.Data;

public class ScimDbContext : DbContext
{
    public ScimDbContext(DbContextOptions<ScimDbContext> options) : base(options)
    {
    }

    public DbSet<ProvisionedUser> Users => Set<ProvisionedUser>();

    public DbSet<ProvisionedUserGroup> UserGroups => Set<ProvisionedUserGroup>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProvisionedUser>(entity =>
        {
            entity.HasIndex(u => u.UserName).IsUnique();
            entity.HasIndex(u => u.Email);
            entity.HasMany(u => u.Groups)
                .WithOne(g => g.ProvisionedUser)
                .HasForeignKey(g => g.ProvisionedUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
