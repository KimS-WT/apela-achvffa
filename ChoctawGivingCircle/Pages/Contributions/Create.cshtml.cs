using System.Security.Claims;
using ChoctawGivingCircle.Data;
using ChoctawGivingCircle.Models;
using ChoctawGivingCircle.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ChoctawGivingCircle.Pages.Contributions;

[AllowAnonymous]
public class CreateModel : PageModel
{
    private readonly IContributionService contributionService;
    private readonly IAssistanceRequestService requestService;
    private readonly ApplicationDbContext dbContext;
    private readonly IEmailService emailService;
    private readonly UserManager<ApplicationUser> userManager;

    public CreateModel(IContributionService contributionService, IAssistanceRequestService requestService, ApplicationDbContext dbContext, IEmailService emailService, UserManager<ApplicationUser> userManager)
    {
        this.contributionService = contributionService;
        this.requestService = requestService;
        this.dbContext = dbContext;
        this.emailService = emailService;
        this.userManager = userManager;
    }

    [BindProperty]
    public Contribution Input { get; set; } = new();

    [BindProperty]
    public bool IsAnonymous { get; set; }

    [BindProperty]
    public string? ContactEmail { get; set; }

    [BindProperty]
    public int? DropOffLocationId { get; set; }

    [BindProperty]
    public DateTime? DropOffDate { get; set; }

    public List<Location> Locations { get; set; } = new();
    public List<AssistanceRequest> Requests { get; set; } = new();

    public async Task OnGetAsync(int? requestId)
    {
        Requests = await requestService.GetOpenRequestsAsync();
        Locations = await dbContext.Locations.Where(l => l.IsActive).ToListAsync();

        if (requestId.HasValue)
        {
            Input.AssistanceRequestId = requestId.Value;
            Input.IsGeneralFund = false;
        }
        else
        {
            Input.IsGeneralFund = true;
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync(Input.AssistanceRequestId);
            return Page();
        }

        // Determine donor
        string? donorId = null;
        if (!IsAnonymous && User?.Identity?.IsAuthenticated == true)
        {
            donorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        Input.DonorUserId = donorId;
        Input.CreatedAt = DateTime.UtcNow;

        // If no assistance request selected and IsGeneralFund not set, default to general fund
        if (Input.AssistanceRequestId == null)
        {
            Input.IsGeneralFund = true;
        }

        await contributionService.CreateAsync(Input);

        // If item and dropoff info provided, create DonationDropOff
        if (DropOffLocationId.HasValue)
        {
            var dropOff = new DonationDropOff
            {
                ContributionId = Input.Id,
                LocationId = DropOffLocationId.Value,
                ScheduledDate = DropOffDate ?? DateTime.UtcNow.AddDays(3),
                Notes = Input.Notes,
                RescheduleToken = Guid.NewGuid().ToString("N").Substring(0, 12),
                CreatedAt = DateTime.UtcNow
            };

            await dbContext.DonationDropOffs.AddAsync(dropOff);
            await dbContext.SaveChangesAsync();

            // Send drop-off scheduled email if contact email provided
            var emailTo = ContactEmail;
            if (string.IsNullOrEmpty(emailTo) && donorId != null)
            {
                var user = await userManager.FindByIdAsync(donorId);
                emailTo = user?.Email;
            }

            if (!string.IsNullOrEmpty(emailTo))
            {
                await emailService.SendDropOffScheduledAsync(emailTo!, dropOff);
            }
        }

        // Send donation confirmation (fake)
        var recipientEmail = ContactEmail;
        if (string.IsNullOrEmpty(recipientEmail) && donorId != null)
        {
            var user = await userManager.FindByIdAsync(donorId);
            recipientEmail = user?.Email;
        }

        if (!string.IsNullOrEmpty(recipientEmail))
        {
            await emailService.SendDonationConfirmationAsync(recipientEmail!, Input);
        }

        TempData["DonationCreated"] = true;
        TempData["Receipt"] = $"DEMO-{DateTime.UtcNow:yyyyMMdd}-{Input.Id:D6}";

        return RedirectToPage("/Index");
    }
}
