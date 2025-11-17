using ChoctawGivingCircle.Models;

namespace ChoctawGivingCircle.Services;

public interface IEmailService
{
    Task<bool> SendDonationConfirmationAsync(string recipientEmail, Contribution donation);
    Task<bool> SendRequestApprovedAsync(string recipientEmail, AssistanceRequest request);
    Task<bool> SendRequestStatusChangeAsync(string recipientEmail, AssistanceRequest request, string oldStatus);
    Task<bool> SendCoApprovalNeededAsync(string adminEmail, FundAllocationApproval allocation);
    Task<bool> SendDropOffScheduledAsync(string email, DonationDropOff dropOff);
    Task<bool> SendDropOffRescheduleReminderAsync(string email, DonationDropOff dropOff, string rescheduleToken);
    Task<bool> SendAllocationApprovedAsync(string recipientEmail, FundAllocationApproval allocation);
    Task<bool> SendAllocationDeniedAsync(string recipientEmail, FundAllocationApproval allocation);
}
