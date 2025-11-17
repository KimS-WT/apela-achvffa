using ChoctawGivingCircle.Data;
using ChoctawGivingCircle.Models;
using Microsoft.EntityFrameworkCore;

namespace ChoctawGivingCircle.Services;

public class FakeEmailService(ApplicationDbContext dbContext, ILogger<FakeEmailService> logger) : IEmailService
{
    public async Task<bool> SendDonationConfirmationAsync(string recipientEmail, Contribution donation)
    {
        var subject = "Donation Confirmation - Choctaw Giving Circle";
        var body = $@"
Thank you for your generous donation to the Choctaw Giving Circle!

Donation Details:
- Amount: ${donation.Amount:F2}
- Type: {donation.ContributionType}
- Date: {DateTime.UtcNow:g}
{(donation.AssistanceRequestId.HasValue ? $"- Directed to Request ID: {donation.AssistanceRequestId}" : "- Directed to General Fund")}

Receipt Number: DEMO-{DateTime.UtcNow:yyyyMMdd}-{donation.Id:D6}

Your contribution makes a direct impact in our community. Thank you!

Choctaw Giving Circle Team
";

        return await LogEmailAsync(recipientEmail, subject, body, "DonationConfirmation", donation.Id, null, null);
    }

    public async Task<bool> SendRequestApprovedAsync(string recipientEmail, AssistanceRequest request)
    {
        var subject = "Your Request Has Been Approved - Choctaw Giving Circle";
        var body = $@"
Great news! Your assistance request has been approved and is now open for community support.

Request Details:
- Title: {request.Title}
- Category: {request.Category}
- Priority: {request.Priority}
- Estimated Cost: ${request.EstimatedCost:F2}

Your request is now visible to our community members who may be able to contribute. You can track contributions and updates on your dashboard.

Thank you for being part of our community!

Choctaw Giving Circle Team
";

        return await LogEmailAsync(recipientEmail, subject, body, "RequestApproved", null, request.Id, request.UserId);
    }

    public async Task<bool> SendRequestStatusChangeAsync(string recipientEmail, AssistanceRequest request, string oldStatus)
    {
        var subject = $"Your Request Status Changed - Choctaw Giving Circle";
        var body = $@"
Your assistance request status has been updated.

Request: {request.Title}
Previous Status: {oldStatus}
New Status: {request.Status}

You can view more details on your dashboard at any time.

Choctaw Giving Circle Team
";

        return await LogEmailAsync(recipientEmail, subject, body, "RequestStatusChange", null, request.Id, request.UserId);
    }

    public async Task<bool> SendCoApprovalNeededAsync(string adminEmail, FundAllocationApproval allocation)
    {
        var contribution = await dbContext.Contributions.FirstOrDefaultAsync(c => c.Id == allocation.ContributionId);
        var request = await dbContext.AssistanceRequests.FirstOrDefaultAsync(r => r.Id == allocation.AssistanceRequestId);

        var subject = "Co-Approval Needed: General Fund Allocation";
        var body = $@"
A general fund allocation requires your co-approval.

Details:
- Contribution Amount: ${contribution?.Amount:F2}
- Request ID: {allocation.AssistanceRequestId}
- Request Title: {request?.Title}
- Initiated By: Admin ID {allocation.RequestedByAdminId}
- Reason: Admin-initiated allocation for their own request (requires co-signature)

Please review and approve/deny this allocation in the admin dashboard.

Choctaw Giving Circle Admin System
";

        return await LogEmailAsync(adminEmail, subject, body, "CoApprovalNeeded", allocation.ContributionId, allocation.AssistanceRequestId, null);
    }

    public async Task<bool> SendDropOffScheduledAsync(string email, DonationDropOff dropOff)
    {
        var location = await dbContext.Locations.FirstOrDefaultAsync(l => l.Id == dropOff.LocationId);
        var contribution = await dbContext.Contributions.FirstOrDefaultAsync(c => c.Id == dropOff.ContributionId);

        var subject = "Drop-Off Scheduled - Choctaw Giving Circle";
        var body = $@"
Your donation drop-off has been scheduled!

Location Details:
- Name: {location?.Name}
- Address: {location?.Address}
- Phone: {location?.Phone}
- Scheduled Date: {dropOff.ScheduledDate:g}

Item Details:
- Type: {contribution?.ContributionType}
- Amount/Quantity: {contribution?.Amount}
- Notes: {dropOff.Notes}

Thank you for your support!

Choctaw Giving Circle Team
";

        return await LogEmailAsync(email, subject, body, "DropOffScheduled", dropOff.ContributionId, null, null);
    }

    public async Task<bool> SendDropOffRescheduleReminderAsync(string email, DonationDropOff dropOff, string rescheduleToken)
    {
        var location = await dbContext.Locations.FirstOrDefaultAsync(l => l.Id == dropOff.LocationId);

        var subject = "Reschedule Your Donation Drop-Off - Choctaw Giving Circle";
        var body = $@"
We need to reschedule your donation drop-off.

Current Scheduled Date: {dropOff.ScheduledDate:g}
Location: {location?.Name}

To reschedule, please visit: https://localhost:5001/Contributions/Reschedule?token={rescheduleToken}

If you have questions, please contact us.

Choctaw Giving Circle Team
";

        return await LogEmailAsync(email, subject, body, "DropOffReschedule", dropOff.ContributionId, null, null);
    }

    public async Task<bool> SendAllocationApprovedAsync(string recipientEmail, FundAllocationApproval allocation)
    {
        var contribution = await dbContext.Contributions.FirstOrDefaultAsync(c => c.Id == allocation.ContributionId);
        var request = await dbContext.AssistanceRequests.FirstOrDefaultAsync(r => r.Id == allocation.AssistanceRequestId);

        var subject = "Allocation Approved - Choctaw Giving Circle";
        var body = $@"
A general fund allocation has been approved.

Details:
- Contribution Amount: ${contribution?.Amount:F2}
- Request ID: {allocation.AssistanceRequestId}
- Request Title: {request?.Title}
- Approved At: {allocation.ApprovedAt:g}

Thank you.
";

        return await LogEmailAsync(recipientEmail, subject, body, "AllocationApproved", allocation.ContributionId, allocation.AssistanceRequestId, null);
    }

    public async Task<bool> SendAllocationDeniedAsync(string recipientEmail, FundAllocationApproval allocation)
    {
        var contribution = await dbContext.Contributions.FirstOrDefaultAsync(c => c.Id == allocation.ContributionId);
        var request = await dbContext.AssistanceRequests.FirstOrDefaultAsync(r => r.Id == allocation.AssistanceRequestId);

        var subject = "Allocation Denied - Choctaw Giving Circle";
        var body = $@"
A general fund allocation request was denied by an administrator.

Details:
- Contribution Amount: ${contribution?.Amount:F2}
- Request ID: {allocation.AssistanceRequestId}
- Request Title: {request?.Title}

If you believe this decision should be reconsidered, contact support.
";

        return await LogEmailAsync(recipientEmail, subject, body, "AllocationDenied", allocation.ContributionId, allocation.AssistanceRequestId, null);
    }

    private async Task<bool> LogEmailAsync(string recipientEmail, string subject, string body, string emailType, int? contributionId = null, int? requestId = null, string? userId = null)
    {
        try
        {
            var emailLog = new EmailLog
            {
                RecipientEmail = recipientEmail,
                Subject = subject,
                Body = body,
                EmailType = emailType,
                Status = "Sent",
                RelatedContributionId = contributionId,
                RelatedAssistanceRequestId = requestId,
                RelatedUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await dbContext.EmailLogs.AddAsync(emailLog);
            await dbContext.SaveChangesAsync();

            // Log to console in development
            logger.LogInformation($"[FAKE EMAIL] To: {recipientEmail}, Subject: {subject}, Type: {emailType}");

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError($"Error logging email: {ex.Message}");
            return false;
        }
    }
}
