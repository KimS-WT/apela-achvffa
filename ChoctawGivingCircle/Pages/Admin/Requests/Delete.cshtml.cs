using ChoctawGivingCircle.Models;
using ChoctawGivingCircle.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChoctawGivingCircle.Pages.Admin.Requests;

[Authorize(Roles = "Admin")]
public class DeleteModel(IAssistanceRequestService requestService) : PageModel
{
    public AssistanceRequest? RequestItem { get; private set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        RequestItem = await requestService.GetByIdAsync(id);
        if (RequestItem == null)
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        await requestService.DeleteAsync(id);
        TempData["StatusMessage"] = "Request removed.";
        return RedirectToPage("/Admin/Requests/Index");
    }
}
