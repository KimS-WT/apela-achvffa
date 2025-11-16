using System.Security.Claims;
using ChoctawGivingCircle.Models;
using ChoctawGivingCircle.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChoctawGivingCircle.Pages.Requests;

[Authorize]
public class MyModel(IAssistanceRequestService requestService) : PageModel
{
    public List<AssistanceRequest> Requests { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        Requests = await requestService.GetUserRequestsAsync(userId);
        return Page();
    }
}
