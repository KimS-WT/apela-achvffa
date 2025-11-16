using ChoctawGivingCircle.Data;
using ChoctawGivingCircle.Models;
using ChoctawGivingCircle.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ChoctawGivingCircle.Pages.Admin.Requests;

[Authorize(Roles = "Admin")]
public class DetailsModel(
    IAssistanceRequestService requestService,
    ApplicationDbContext dbContext) : PageModel
{
    public AssistanceRequest? RequestItem { get; private set; }
    public List<Contribution> Contributions { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var request = await requestService.GetByIdAsync(id);
        if (request == null)
        {
            return NotFound();
        }

        RequestItem = request;
        Contributions = await dbContext.Contributions
            .Where(c => c.AssistanceRequestId == id)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return Page();
    }
}
