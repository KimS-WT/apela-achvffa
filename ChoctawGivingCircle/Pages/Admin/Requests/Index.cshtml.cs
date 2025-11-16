using ChoctawGivingCircle.Models;
using ChoctawGivingCircle.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChoctawGivingCircle.Pages.Admin.Requests;

[Authorize(Roles = "Admin")]
public class IndexModel(IAssistanceRequestService requestService) : PageModel
{
    public List<AssistanceRequest> Requests { get; private set; } = [];

    public async Task OnGetAsync()
    {
        Requests = await requestService.GetAllAsync();
    }
}
