using ChoctawGivingCircle.Models;
using ChoctawGivingCircle.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ChoctawGivingCircle.Pages.Admin.Contributions;

[Authorize(Roles = "Admin")]
public class AllocateModel(
    IContributionService contributionService,
    IAssistanceRequestService assistanceRequestService) : PageModel
{
    public Contribution? Contribution { get; private set; }
    public SelectList RequestOptions { get; private set; } = default!;

    [BindProperty]
    public int SelectedRequestId { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var contribution = await contributionService.GetByIdAsync(id);
        if (contribution == null || !contribution.IsGeneralFund)
        {
            return NotFound();
        }

        Contribution = contribution;
        await LoadRequestOptionsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var contribution = await contributionService.GetByIdAsync(id);
        if (contribution == null || !contribution.IsGeneralFund)
        {
            return NotFound();
        }

        if (SelectedRequestId <= 0)
        {
            ModelState.AddModelError(nameof(SelectedRequestId), "Select a request to allocate this contribution.");
        }

        if (!ModelState.IsValid)
        {
            Contribution = contribution;
            await LoadRequestOptionsAsync();
            return Page();
        }

        await contributionService.AllocateToRequestAsync(id, SelectedRequestId);
        TempData["StatusMessage"] = "Contribution allocated to request.";
        return RedirectToPage("/Admin/Contributions/Index");
    }

    private async Task LoadRequestOptionsAsync()
    {
        var openRequests = await assistanceRequestService.GetOpenRequestsAsync();
        RequestOptions = new SelectList(openRequests, "Id", "Title");
        if (openRequests.Count > 0)
        {
            SelectedRequestId = openRequests[0].Id;
        }
    }
}
