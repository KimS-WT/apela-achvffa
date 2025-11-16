using ChoctawGivingCircle.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ChoctawGivingCircle.Pages.Profile;

[Authorize]
public class IndexModel(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ApplicationDbContext dbContext) : PageModel
{
    [BindProperty]
    public ProfileInputModel Input { get; set; } = new();

    public SelectList TribeOptions { get; private set; } = default!;

    [TempData]
    public string? StatusMessage { get; set; }

    public class ProfileInputModel
    {
        public string? DisplayName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Bio { get; set; }

        public bool IsTribalMember { get; set; }

        public int? TribeId { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound("Unable to load current user.");
        }

        Input = new ProfileInputModel
        {
            DisplayName = user.DisplayName,
            PhoneNumber = user.PhoneNumber,
            Bio = user.Bio,
            IsTribalMember = user.IsTribalMember,
            TribeId = user.TribeId
        };

        await LoadTribesAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound("Unable to load current user.");
        }

        if (!ModelState.IsValid)
        {
            await LoadTribesAsync();
            return Page();
        }

        user.DisplayName = Input.DisplayName;
        user.PhoneNumber = Input.PhoneNumber;
        user.Bio = Input.Bio;
        user.IsTribalMember = Input.IsTribalMember;
        user.TribeId = Input.TribeId;

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            await LoadTribesAsync();
            return Page();
        }

        await signInManager.RefreshSignInAsync(user);
        StatusMessage = "Profile updated.";
        return RedirectToPage();
    }

    private async Task LoadTribesAsync()
    {
        var tribes = await dbContext.Tribes
            .OrderBy(t => t.Name)
            .Select(t => new { t.Id, t.Name })
            .ToListAsync();

        TribeOptions = new SelectList(tribes, "Id", "Name");
    }
}
