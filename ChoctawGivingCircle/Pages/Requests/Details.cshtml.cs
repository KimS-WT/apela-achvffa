using ChoctawGivingCircle.Models;
using ChoctawGivingCircle.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChoctawGivingCircle.Pages.Requests;

public class DetailsModel(IAssistanceRequestService requestService) : PageModel
{
    public AssistanceRequest? Item { get; private set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Item = await requestService.GetByIdAsync(id);
        if (Item == null)
        {
            return NotFound();
        }

        return Page();
    }
}
