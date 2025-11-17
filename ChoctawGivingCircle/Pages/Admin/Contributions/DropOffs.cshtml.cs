using ChoctawGivingCircle.Data;
using ChoctawGivingCircle.Models;
using ChoctawGivingCircle.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace ChoctawGivingCircle.Pages.Admin.Contributions;

[Authorize(Roles = "Admin")]
public class DropOffsModel(ApplicationDbContext dbContext, IEmailService emailService, UserManager<ApplicationUser> userManager) : PageModel
{
    private readonly ApplicationDbContext _db = dbContext;
    private readonly IEmailService _emailService = emailService;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public List<DonationDropOff> DropOffList { get; private set; } = new();

    public async Task OnGetAsync()
    {
        DropOffList = await _db.DonationDropOffs
            .Include(d => d.Contribution)
            .Include(d => d.Location)
            .OrderBy(d => d.ScheduledDate)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostCompleteAsync(int id)
    {
        var drop = await _db.DonationDropOffs.FindAsync(id);
        if (drop == null)
            return NotFound();

        drop.IsCompleted = true;
        drop.CompletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        TempData["StatusMessage"] = "Drop-off marked completed.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRescheduleAsync(int id, DateTime newDate)
    {
        var drop = await _db.DonationDropOffs.FindAsync(id);
        if (drop == null)
            return NotFound();

        drop.ScheduledDate = newDate;
        drop.RescheduleToken = Guid.NewGuid().ToString("N").Substring(0, 12);
        await _db.SaveChangesAsync();

        // Notify donor if possible
        var contribution = await _db.Contributions.FindAsync(drop.ContributionId);
        string? email = null;
        if (contribution != null && !string.IsNullOrEmpty(contribution.DonorUserId))
        {
            var user = await _userManager.FindByIdAsync(contribution.DonorUserId);
            email = user?.Email;
        }

        if (!string.IsNullOrEmpty(email))
        {
            await _emailService.SendDropOffRescheduleReminderAsync(email, drop, drop.RescheduleToken!);
        }

        TempData["StatusMessage"] = "Drop-off rescheduled and donor notified.";
        return RedirectToPage();
    }
}
