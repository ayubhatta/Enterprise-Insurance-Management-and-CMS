using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Enterprise_Insurance_Management___CMS_Platform.Entities;

namespace Enterprise_Insurance_Management___CMS_Platform.Data;
public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Policy> Policies { get; set; } = null!;
    public DbSet<ClaimEntity> Claims { get; set; } = null!;
    public DbSet<DocumentEntity> Documents { get; set; } = null!;
    public DbSet<CustomerProfile> CustomerProfiles { get; set; }
    public DbSet<CustomerPolicy> CustomerPolicies { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<CustomerPolicy>()
            .HasIndex(cp => new { cp.CustomerId, cp.PolicyId })
            .IsUnique();

        builder.Entity<CustomerPolicy>()
            .HasOne(cp => cp.Customer)
            .WithMany() 
            .HasForeignKey(cp => cp.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CustomerPolicy>()
            .HasOne(cp => cp.Policy)
            .WithMany(p => p.CustomerPolicies)
            .HasForeignKey(cp => cp.PolicyId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
