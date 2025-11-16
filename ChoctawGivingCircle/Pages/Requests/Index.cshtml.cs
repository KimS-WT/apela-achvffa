using ChoctawGivingCircle.Models;
using ChoctawGivingCircle.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChoctawGivingCircle.Pages.Requests;

public class IndexModel(IAssistanceRequestService requestService) : PageModel
{
    public List<AssistanceRequest> Requests { get; private set; } = [];

    public async Task OnGetAsync()
    {
        Requests = await requestService.GetOpenRequestsAsync();
    }
}
