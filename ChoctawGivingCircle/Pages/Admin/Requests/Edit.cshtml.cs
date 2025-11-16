using ChoctawGivingCircle.Models;
using ChoctawGivingCircle.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChoctawGivingCircle.Pages.Admin.Requests;

[Authorize(Roles = "Admin")]
public class EditModel(IAssistanceRequestService requestService) : PageModel
{
    [BindProperty]
    public AssistanceRequest Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var request = await requestService.GetByIdAsync(id);
        if (request == null)
        {
            return NotFound();
        }

        Input = request;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await requestService.UpdateAsync(Input);
        TempData["StatusMessage"] = "Request updated.";
        return RedirectToPage("/Admin/Requests/Index");
    }
}
