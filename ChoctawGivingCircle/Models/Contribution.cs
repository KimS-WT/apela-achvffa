using System;
using System.ComponentModel.DataAnnotations;
using ChoctawGivingCircle.Data;

namespace ChoctawGivingCircle.Models;

public class Contribution
{
    public int Id { get; set; }
    public int? AssistanceRequestId { get; set; }
    public string? DonorUserId { get; set; }
    public bool IsGeneralFund { get; set; }
    public decimal Amount { get; set; }
    [StringLength(120)]
    public string ContributionType { get; set; } = "Money";
    [StringLength(500)]
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }

    public AssistanceRequest? AssistanceRequest { get; set; }
    public ApplicationUser? DonorUser { get; set; }
}
