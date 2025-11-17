using System.Security.Claims;
using ChoctawGivingCircle.Models;
using ChoctawGivingCircle.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChoctawGivingCircle.Pages.Requests;

[Authorize]
public class CreateModel(IAssistanceRequestService requestService) : PageModel
{
    [BindProperty]
    public AssistanceRequest Input { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        Input.UserId = userId;
        Input.Status = AssistanceStatus.Submitted;

        // We assign UserId server-side; remove any prior ModelState entry so validation re-evaluates with the user id present.
        ModelState.Remove("Input.UserId");
        ModelState.Remove("UserId");

        if (!ModelState.IsValid)
        {
            // collect model state errors for debugging/feedback
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).Where(m => !string.IsNullOrEmpty(m));
            if (errors.Any())
            {
                TempData["ModelErrors"] = string.Join(" | ", errors);
            }

            return Page();
        }

        try
        {
            await requestService.CreateAsync(Input);
            TempData["SuccessMessage"] = "Your assistance request has been submitted successfully! It will be reviewed by our admin team.";
            return RedirectToPage("/Requests/My");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error creating request: {ex.Message}");
            return Page();
        }
    }
}
