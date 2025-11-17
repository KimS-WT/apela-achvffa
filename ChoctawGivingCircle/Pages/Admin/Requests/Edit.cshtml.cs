using ChoctawGivingCircle.Models;
using ChoctawGivingCircle.Services;
using ChoctawGivingCircle.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;


namespace ChoctawGivingCircle.Pages.Admin.Requests;

[Authorize(Roles = "Admin")]
public class EditModel(IAssistanceRequestService requestService, ApplicationDbContext dbContext, IEmailService emailService) : PageModel
{
    [BindProperty]
    public AssistanceRequest Input { get; set; } = new();
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly IEmailService _emailService = emailService;

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

        // Load existing request to detect status changes
        var existing = await requestService.GetByIdAsync(Input.Id);
        var oldStatus = existing?.Status;

        await requestService.UpdateAsync(Input);

        // If status changed, send notification to request owner (if email available)
        if (oldStatus != null && oldStatus != Input.Status)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == Input.UserId);
            var email = user?.Email;
            if (!string.IsNullOrEmpty(email))
            {
                // Send general status change email
                await _emailService.SendRequestStatusChangeAsync(email, Input, oldStatus.ToString());

                // If newly approved/open, send approved email as well
                if (Input.Status == AssistanceStatus.Approved || Input.Status == AssistanceStatus.Open)
                {
                    await _emailService.SendRequestApprovedAsync(email, Input);
                }
            }
        }

        TempData["StatusMessage"] = "Request updated.";
        return RedirectToPage("/Admin/Requests/Index");
    }
}
