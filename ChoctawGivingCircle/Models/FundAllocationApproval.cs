using System.ComponentModel.DataAnnotations;
using ChoctawGivingCircle.Data;

namespace ChoctawGivingCircle.Models;

public enum AllocationApprovalStatus
{
    Pending,
    Approved,
    Denied
}

public class FundAllocationApproval
{
    public int Id { get; set; }

    [Required]
    public int ContributionId { get; set; }

    [Required]
    public int AssistanceRequestId { get; set; }

    [Required]
    [StringLength(150)]
    public string RequestedByAdminId { get; set; } = string.Empty;

    [StringLength(150)]
    public string? ApprovedByAdminId { get; set; }

    public AllocationApprovalStatus Status { get; set; } = AllocationApprovalStatus.Pending;

    [StringLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }

    // Navigation
    public Contribution? Contribution { get; set; }
    public AssistanceRequest? AssistanceRequest { get; set; }
    public ApplicationUser? RequestedByAdmin { get; set; }
    public ApplicationUser? ApprovedByAdmin { get; set; }
}
