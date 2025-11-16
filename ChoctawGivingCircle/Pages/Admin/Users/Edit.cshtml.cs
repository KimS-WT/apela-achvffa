using ChoctawGivingCircle.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ChoctawGivingCircle.Pages.Admin.Users;

[Authorize(Roles = "Admin")]
public class EditModel(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager) : PageModel
{
    [BindProperty]
    public UserInputModel Input { get; set; } = new();

    public List<string> AllRoles { get; private set; } = [];

    public class UserInputModel
    {
        public string Id { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Bio { get; set; }
        public bool IsTribalMember { get; set; }
        public int? TribeId { get; set; }
        public List<string> Roles { get; set; } = [];
    }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        AllRoles = await roleManager.Roles.Select(r => r.Name ?? string.Empty).ToListAsync();

        Input = new UserInputModel
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Bio = user.Bio,
            IsTribalMember = user.IsTribalMember,
            TribeId = user.TribeId,
            Roles = (await userManager.GetRolesAsync(user)).ToList()
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await userManager.FindByIdAsync(Input.Id);
        if (user == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            AllRoles = await roleManager.Roles.Select(r => r.Name ?? string.Empty).ToListAsync();
            return Page();
        }

        user.DisplayName = Input.DisplayName;
        user.PhoneNumber = Input.PhoneNumber;
        user.Bio = Input.Bio;
        user.IsTribalMember = Input.IsTribalMember;
        user.TribeId = Input.TribeId;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            AllRoles = await roleManager.Roles.Select(r => r.Name ?? string.Empty).ToListAsync();
            return Page();
        }

        var roles = await userManager.GetRolesAsync(user);
        var rolesToAdd = Input.Roles.Except(roles);
        var rolesToRemove = roles.Except(Input.Roles);

        if (rolesToAdd.Any())
        {
            await userManager.AddToRolesAsync(user, rolesToAdd);
        }

        if (rolesToRemove.Any())
        {
            await userManager.RemoveFromRolesAsync(user, rolesToRemove);
        }

        TempData["StatusMessage"] = "User updated.";
        return RedirectToPage("/Admin/Users/Index");
    }
}
