using ChoctawGivingCircle.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChoctawGivingCircle.Pages.Admin.Users;

[Authorize(Roles = "Admin")]
public class DeleteModel(UserManager<ApplicationUser> userManager) : PageModel
{
    public ApplicationUser? TargetUser { get; private set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        TargetUser = await userManager.FindByIdAsync(id);
        if (TargetUser == null)
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        await userManager.DeleteAsync(user);
        TempData["StatusMessage"] = "User removed.";
        return RedirectToPage("/Admin/Users/Index");
    }
}
