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
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        Input.UserId = userId;
        Input.Status = AssistanceStatus.Submitted;

        await requestService.CreateAsync(Input);
        return RedirectToPage("/Requests/My");
    }
}
