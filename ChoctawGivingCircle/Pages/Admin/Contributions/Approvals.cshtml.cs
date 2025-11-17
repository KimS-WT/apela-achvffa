using ChoctawGivingCircle.Data;
using ChoctawGivingCircle.Models;
using ChoctawGivingCircle.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace ChoctawGivingCircle.Pages.Admin.Contributions;

[Authorize(Roles = "Admin")]
public class ApprovalsModel(ApplicationDbContext dbContext, IContributionService contributionService, IEmailService emailService, UserManager<ApplicationUser> userManager) : PageModel
{
    private readonly ApplicationDbContext _db = dbContext;
    private readonly IContributionService _contributionService = contributionService;
    private readonly IEmailService _emailService = emailService;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public List<FundAllocationApproval> Pending { get; private set; } = new();

    public async Task OnGetAsync()
    {
        Pending = await _db.FundAllocationApprovals
            .Where(a => a.Status == AllocationApprovalStatus.Pending)
            .Include(a => a.Contribution)
            .Include(a => a.RequestedByAdmin)
            .Include(a => a.AssistanceRequest)
            .OrderBy(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostApproveAsync(int id)
    {
        var approval = await _db.FundAllocationApprovals.FindAsync(id);
        if (approval == null || approval.Status != AllocationApprovalStatus.Pending)
            return NotFound();

        var currentAdminId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // perform allocation
        await _contributionService.AllocateToRequestAsync(approval.ContributionId, approval.AssistanceRequestId);

        approval.Status = AllocationApprovalStatus.Approved;
        approval.ApprovedByAdminId = currentAdminId;
        approval.ApprovedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        // notify requester and initiating admin
        var request = await _db.AssistanceRequests.FindAsync(approval.AssistanceRequestId);
        var requester = request != null ? await _userManager.FindByIdAsync(request.UserId) : null;
        var initiator = !string.IsNullOrEmpty(approval.RequestedByAdminId) ? await _userManager.FindByIdAsync(approval.RequestedByAdminId) : null;

        if (requester != null && !string.IsNullOrEmpty(requester.Email))
        {
            await _emailService.SendAllocationApprovedAsync(requester.Email, approval);
        }

        if (initiator != null && !string.IsNullOrEmpty(initiator.Email))
        {
            await _emailService.SendAllocationApprovedAsync(initiator.Email, approval);
        }

        TempData["StatusMessage"] = "Allocation approved and applied.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDenyAsync(int id)
    {
        var approval = await _db.FundAllocationApprovals.FindAsync(id);
        if (approval == null || approval.Status != AllocationApprovalStatus.Pending)
            return NotFound();

        var currentAdminId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        approval.Status = AllocationApprovalStatus.Denied;
        approval.ApprovedByAdminId = currentAdminId;
        approval.ApprovedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        // notify initiator
        var initiator = !string.IsNullOrEmpty(approval.RequestedByAdminId) ? await _userManager.FindByIdAsync(approval.RequestedByAdminId) : null;
        if (initiator != null && !string.IsNullOrEmpty(initiator.Email))
        {
            await _emailService.SendAllocationDeniedAsync(initiator.Email, approval);
        }

        TempData["StatusMessage"] = "Allocation denied.";
        return RedirectToPage();
    }
}
