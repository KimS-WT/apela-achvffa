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
        var userManager = scopedProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var adminUser = await EnsureAdminAsync(userManager);
        await SeedTribesAsync(dbContext);
        await SeedCommunityMembersAsync(userManager);
        await SeedSampleRequestsAsync(dbContext, adminUser.Id);
        await SeedSampleContributionsAsync(dbContext, userManager);
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
            IsTribalMember = true,
            DisplayName = "Demo Admin",
            Bio = "Platform administrator ensuring every request stays on mission."
        };

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

    private static async Task SeedCommunityMembersAsync(UserManager<ApplicationUser> userManager)
    {
        var seedUsers = new List<(ApplicationUser User, string Password, string[] Roles)>
        {
            (
                new ApplicationUser
                {
                    UserName = "supporter1@demo.local",
                    Email = "supporter1@demo.local",
                    EmailConfirmed = true,
                    DisplayName = "River Stone",
                    Bio = "Choctaw descendant focused on supporting education journeys.",
                    IsTribalMember = false,
                    PhoneNumber = "405-555-1122"
                },
                "Supporter!123",
                new[] { "Donor" }
            ),
            (
                new ApplicationUser
                {
                    UserName = "supporter2@demo.local",
                    Email = "supporter2@demo.local",
                    EmailConfirmed = true,
                    DisplayName = "Cedar Flame",
                    Bio = "Community advocate offering transport and mentorship.",
                    IsTribalMember = true,
                    PhoneNumber = "918-555-7788"
                },
                "Supporter!123",
                new[] { "Donor", "Requester" }
            ),
            (
                new ApplicationUser
                {
                    UserName = "requester1@demo.local",
                    Email = "requester1@demo.local",
                    EmailConfirmed = true,
                    DisplayName = "Sky Willow",
                    Bio = "Student preparing regalia for upcoming stomp dance.",
                    IsTribalMember = true,
                    PhoneNumber = "580-555-3344"
                },
                "Requester!123",
                new[] { "Requester" }
            ),
            (
                new ApplicationUser
                {
                    UserName = "requester2@demo.local",
                    Email = "requester2@demo.local",
                    EmailConfirmed = true,
                    DisplayName = "Stone Bear",
                    Bio = "Caregiver seeking travel support for elder appointments.",
                    IsTribalMember = true,
                    PhoneNumber = "601-555-9911"
                },
                "Requester!123",
                new[] { "Requester" }
            ),
            (
                new ApplicationUser
                {
                    UserName = "admin.supporter@demo.local",
                    Email = "admin.supporter@demo.local",
                    EmailConfirmed = true,
                    DisplayName = "Echo Hawk",
                    Bio = "Admin steward focused on donor outreach and onboarding.",
                    IsTribalMember = false,
                    PhoneNumber = "214-555-6677"
                },
                "AdminSupport!123",
                new[] { "Admin", "Donor" }
            ),
            (
                new ApplicationUser
                {
                    UserName = "admin.requester@demo.local",
                    Email = "admin.requester@demo.local",
                    EmailConfirmed = true,
                    DisplayName = "Winter Rain",
                    Bio = "Admin liaison advocating for frontline caregivers.",
                    IsTribalMember = true,
                    PhoneNumber = "405-555-9900"
                },
                "AdminRequest!123",
                new[] { "Admin", "Requester" }
            )
        };

        foreach (var (user, password, roles) in seedUsers)
        {
            if (await userManager.FindByEmailAsync(user.Email!) != null)
            {
                continue;
            }

            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException($"Failed to create seed user {user.Email}: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
            }

            foreach (var role in roles)
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
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

    private static async Task SeedSampleContributionsAsync(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        if (await dbContext.Contributions.AnyAsync())
        {
            return;
        }

        var supporter1 = await userManager.FindByEmailAsync("supporter1@demo.local");
        var supporter2 = await userManager.FindByEmailAsync("supporter2@demo.local");

        var requests = await dbContext.AssistanceRequests.OrderBy(r => r.Id).ToListAsync();
        if (requests.Count == 0)
        {
            return;
        }

        var contributions = new List<Contribution>
        {
            new()
            {
                AssistanceRequestId = requests.First().Id,
                Amount = 250m,
                ContributionType = "Money",
                DonorUserId = supporter1?.Id,
                Notes = "Covering textbook costs ASAP.",
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                IsGeneralFund = false
            }
        };

        if (requests.Count > 1)
        {
            contributions.Add(new Contribution
            {
                AssistanceRequestId = requests[1].Id,
                Amount = 125m,
                ContributionType = "Item",
                DonorUserId = supporter2?.Id,
                Notes = "Partial regalia materials sourced from local artisans.",
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                IsGeneralFund = false
            });
        }

        contributions.Add(new Contribution
        {
            Amount = 500m,
            ContributionType = "Money",
            DonorUserId = supporter2?.Id,
            Notes = "General fund for urgent needs.",
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            IsGeneralFund = true
        });

        await dbContext.Contributions.AddRangeAsync(contributions);
        await dbContext.SaveChangesAsync();
    }
}
