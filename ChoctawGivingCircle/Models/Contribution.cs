using System;

namespace ChoctawGivingCircle.Models;

public class Contribution
{
    public int Id { get; set; }
    public int AssistanceRequestId { get; set; }
    public string? DonorUserId { get; set; }
    public decimal Amount { get; set; }
    public string ContributionType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public AssistanceRequest? AssistanceRequest { get; set; }
}
