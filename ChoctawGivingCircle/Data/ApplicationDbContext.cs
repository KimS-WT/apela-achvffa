using ChoctawGivingCircle.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChoctawGivingCircle.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<AssistanceRequest> AssistanceRequests => Set<AssistanceRequest>();
    public DbSet<Contribution> Contributions => Set<Contribution>();
    public DbSet<Tribe> Tribes => Set<Tribe>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<DonationDropOff> DonationDropOffs => Set<DonationDropOff>();
    public DbSet<EmailLog> EmailLogs => Set<EmailLog>();
    public DbSet<FundAllocationApproval> FundAllocationApprovals => Set<FundAllocationApproval>();

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    private void UpdateTimestamps()
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<AssistanceRequest>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;
            }

            if (entry.State == EntityState.Modified || entry.State == EntityState.Added)
            {
                entry.Entity.UpdatedAt = utcNow;
            }
        }

        foreach (var entry in ChangeTracker.Entries<Contribution>())
        {
            if (entry.State == EntityState.Added && entry.Entity.CreatedAt == default)
            {
                entry.Entity.CreatedAt = utcNow;
            }
        }

        foreach (var entry in ChangeTracker.Entries<Location>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;
            }

            if (entry.State == EntityState.Modified || entry.State == EntityState.Added)
            {
                entry.Entity.UpdatedAt = utcNow;
            }
        }

        foreach (var entry in ChangeTracker.Entries<DonationDropOff>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;
            }

            if (entry.State == EntityState.Modified || entry.State == EntityState.Added)
            {
                entry.Entity.UpdatedAt = utcNow;
            }
        }

        foreach (var entry in ChangeTracker.Entries<EmailLog>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;
            }
        }

        foreach (var entry in ChangeTracker.Entries<FundAllocationApproval>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;
            }
        }
    }
}
