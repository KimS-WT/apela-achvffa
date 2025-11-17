using ChoctawGivingCircle.Models;
using ChoctawGivingCircle.Services;
using ChoctawGivingCircle.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ChoctawGivingCircle.Pages.Requests;

public class DetailsModel(IAssistanceRequestService requestService, ApplicationDbContext dbContext) : PageModel
{
    public AssistanceRequest? Item { get; private set; }

    public List<Contribution> Contributions { get; private set; } = new();

    public decimal FundedAmount { get; private set; }

    public int ProgressPercent { get; private set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Item = await requestService.GetByIdAsync(id);
        if (Item == null)
        {
            return NotFound();
        }

        // Load contributions directed to this request (including allocated general fund contributions)
        Contributions = await dbContext.Contributions
            .Where(c => c.AssistanceRequestId == id)
            .Include(c => c.DonorUser)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        FundedAmount = Contributions.Sum(c => c.Amount);

        if (Item.EstimatedCost > 0)
        {
            ProgressPercent = (int)Math.Min(100, Math.Round((double)(FundedAmount / Item.EstimatedCost) * 100));
        }
        else
        {
            ProgressPercent = FundedAmount > 0 ? 100 : 0;
        }

        return Page();
    }
}
