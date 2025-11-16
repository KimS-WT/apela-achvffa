using ChoctawGivingCircle.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ChoctawGivingCircle.Pages.Admin.Users;

[Authorize(Roles = "Admin")]
public class IndexModel(UserManager<ApplicationUser> userManager) : PageModel
{
    public List<UserSummaryViewModel> Users { get; private set; } = [];

    public class UserSummaryViewModel
    {
        public string Id { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string? DisplayName { get; init; }
        public bool IsTribalMember { get; init; }
        public string Roles { get; init; } = string.Empty;
    }

    public async Task OnGetAsync()
    {
        var allUsers = await userManager.Users
            .OrderBy(u => u.Email)
            .ToListAsync();

        var result = new List<UserSummaryViewModel>(allUsers.Count);
        foreach (var user in allUsers)
        {
            var roles = await userManager.GetRolesAsync(user);
            result.Add(new UserSummaryViewModel
            {
                Id = user.Id,
                Email = user.Email ?? user.UserName ?? "Unknown",
                DisplayName = user.DisplayName,
                IsTribalMember = user.IsTribalMember,
                Roles = roles.Count > 0 ? string.Join(", ", roles) : "Requester"
            });
        }

        Users = result;
    }
}
