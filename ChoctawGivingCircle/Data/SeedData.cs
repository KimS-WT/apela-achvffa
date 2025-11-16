using ChoctawGivingCircle.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChoctawGivingCircle.Data;

public static class SeedData
{
    private static readonly string[] Roles = ["Requester", "Donor", "Admin"];
    private const string AdminEmail = "admin@demo.local";
    private const string AdminPassword = "Admin!123";

    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var scopedProvider = scope.ServiceProvider;

        var dbContext = scopedProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();

        await EnsureRolesAsync(scopedProvider.GetRequiredService<RoleManager<IdentityRole>>());
        var adminUser = await EnsureAdminAsync(scopedProvider.GetRequiredService<UserManager<ApplicationUser>>());
        await SeedTribesAsync(dbContext);
        await SeedSampleRequestsAsync(dbContext, adminUser.Id);
    }

    private static async Task EnsureRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        foreach (var roleName in Roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }

    private static async Task<ApplicationUser> EnsureAdminAsync(UserManager<ApplicationUser> userManager)
    {
        var adminUser = await userManager.FindByEmailAsync(AdminEmail);
        if (adminUser != null)
        {
            return adminUser;
        }

        adminUser = new ApplicationUser
        {
            UserName = AdminEmail,
            Email = AdminEmail,
            EmailConfirmed = true,
            IsTribalMember = true
        };

        adminUser.DisplayName = "Demo Admin";

        var result = await userManager.CreateAsync(adminUser, AdminPassword);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        await userManager.AddToRoleAsync(adminUser, "Admin");
        return adminUser;
    }

    private static async Task SeedTribesAsync(ApplicationDbContext dbContext)
    {
        if (await dbContext.Tribes.AnyAsync())
        {
            return;
        }

        var tribes = new List<Tribe>
        {
            new() { Name = "Choctaw Nation of Oklahoma", Abbreviation = "CNO" },
            new() { Name = "Mississippi Band of Choctaw Indians", Abbreviation = "MBCI" },
            new() { Name = "Jena Band of Choctaw Indians", Abbreviation = "JBCI" }
        };

        await dbContext.Tribes.AddRangeAsync(tribes);
        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedSampleRequestsAsync(ApplicationDbContext dbContext, string userId)
    {
        if (await dbContext.AssistanceRequests.AnyAsync())
        {
            return;
        }

        var requests = new List<AssistanceRequest>
        {
            new()
            {
                UserId = userId,
                Title = "STEM Textbooks for Spring Semester",
                Description = "Need assistance covering the cost of specialized STEM textbooks for college courses.",
                Category = AssistanceCategory.Education,
                EstimatedCost = 450m,
                Priority = AssistancePriority.High,
                Status = AssistanceStatus.Open,
                NeededByDate = DateTime.UtcNow.AddMonths(1)
            },
            new()
            {
                UserId = userId,
                Title = "Regalia Materials for Powwow",
                Description = "Looking for help obtaining beads and fabric to complete traditional regalia.",
                Category = AssistanceCategory.Regalia,
                EstimatedCost = 275m,
                Priority = AssistancePriority.Medium,
                Status = AssistanceStatus.Approved,
                NeededByDate = DateTime.UtcNow.AddMonths(2)
            },
            new()
            {
                UserId = userId,
                Title = "Elder Care Travel Support",
                Description = "Funding needed for monthly travel to assist an elder relative with medical appointments.",
                Category = AssistanceCategory.Health,
                EstimatedCost = 600m,
                Priority = AssistancePriority.Critical,
                Status = AssistanceStatus.Open
            }
        };

        await dbContext.AssistanceRequests.AddRangeAsync(requests);
        await dbContext.SaveChangesAsync();
    }
}
