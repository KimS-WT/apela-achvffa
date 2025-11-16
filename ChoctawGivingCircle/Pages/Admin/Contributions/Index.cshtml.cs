using ChoctawGivingCircle.Models;
using ChoctawGivingCircle.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChoctawGivingCircle.Pages.Admin.Contributions;

[Authorize(Roles = "Admin")]
public class IndexModel(IContributionService contributionService) : PageModel
{
    public List<Contribution> Contributions { get; private set; } = [];
    public decimal GeneralFundBalance { get; private set; }

    public async Task OnGetAsync()
    {
        Contributions = await contributionService.GetAllAsync();
        GeneralFundBalance = await contributionService.GetGeneralFundBalanceAsync();
    }
}
