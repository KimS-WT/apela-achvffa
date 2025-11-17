using ChoctawGivingCircle.Models;
using ChoctawGivingCircle.Services;
using ChoctawGivingCircle.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ChoctawGivingCircle.Pages.Admin.Contributions;

[Authorize(Roles = "Admin")]
public class AllocateModel(
    IContributionService contributionService,
    IAssistanceRequestService assistanceRequestService,
    ApplicationDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    IEmailService emailService) : PageModel
{
    public Contribution? Contribution { get; private set; }
    public SelectList RequestOptions { get; private set; } = default!;

    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IEmailService _emailService = emailService;

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

        // Load target request to check requester
        var targetRequest = await assistanceRequestService.GetByIdAsync(SelectedRequestId);
        var currentAdminId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (targetRequest != null && targetRequest.UserId == currentAdminId)
        {
            // Admin is allocating to their own request -> require co-approval
            var allocation = new FundAllocationApproval
            {
                ContributionId = contribution.Id,
                AssistanceRequestId = SelectedRequestId,
                RequestedByAdminId = currentAdminId ?? string.Empty,
                Status = AllocationApprovalStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _dbContext.FundAllocationApprovals.AddAsync(allocation);
            await _dbContext.SaveChangesAsync();

            // Notify a random eligible admin (exclude the requester/admin who created this allocation)
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            var eligible = admins.Where(a => a.Id != currentAdminId).ToList();
            ApplicationUser? notifyAdmin = null;
            if (eligible.Count == 0 && admins.Count > 0)
            {
                notifyAdmin = admins.First();
            }
            else if (eligible.Count > 0)
            {
                var rnd = new Random();
                notifyAdmin = eligible[rnd.Next(eligible.Count)];
            }

            if (notifyAdmin != null && !string.IsNullOrEmpty(notifyAdmin.Email))
            {
                await _emailService.SendCoApprovalNeededAsync(notifyAdmin.Email, allocation);
            }

            TempData["StatusMessage"] = "Allocation submitted and requires co-approval from another admin.";
            return RedirectToPage("/Admin/Contributions/Index");
        }

        // Otherwise, proceed to allocate immediately and record approval
        await contributionService.AllocateToRequestAsync(id, SelectedRequestId);

        var approvedRecord = new FundAllocationApproval
        {
            ContributionId = contribution.Id,
            AssistanceRequestId = SelectedRequestId,
            RequestedByAdminId = currentAdminId ?? string.Empty,
            ApprovedByAdminId = currentAdminId,
            Status = AllocationApprovalStatus.Approved,
            CreatedAt = DateTime.UtcNow,
            ApprovedAt = DateTime.UtcNow
        };

        await _dbContext.FundAllocationApprovals.AddAsync(approvedRecord);
        await _dbContext.SaveChangesAsync();

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
