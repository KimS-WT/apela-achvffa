using ChoctawGivingCircle.Data;
using ChoctawGivingCircle.Models;
using ChoctawGivingCircle.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ChoctawGivingCircle.Pages.Admin.Contributions;

[Authorize(Roles = "Admin")]
public class EditModel(IContributionService contributionService, ApplicationDbContext dbContext, IAssistanceRequestService requestService) : PageModel
{
    private readonly IContributionService _contributionService = contributionService;
    private readonly ApplicationDbContext _db = dbContext;
    private readonly IAssistanceRequestService _requestService = requestService;

    [BindProperty]
    public Contribution Input { get; set; } = new();

    public SelectList RequestOptions { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var contribution = await _contributionService.GetByIdAsync(id);
        if (contribution == null)
            return NotFound();

        Input = contribution;
        await LoadRequestsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadRequestsAsync();
            return Page();
        }

        await _contributionService.UpdateAsync(Input);

        TempData["StatusMessage"] = "Contribution updated.";
        return RedirectToPage("/Admin/Contributions/Index");
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _contributionService.DeleteAsync(id);
        TempData["StatusMessage"] = "Contribution deleted.";
        return RedirectToPage("/Admin/Contributions/Index");
    }

    private async Task LoadRequestsAsync()
    {
        var open = await _requestService.GetOpenRequestsAsync();
        RequestOptions = new SelectList(open, "Id", "Title");
    }
}
