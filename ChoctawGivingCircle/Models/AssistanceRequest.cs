using System;
using System.ComponentModel.DataAnnotations;

namespace ChoctawGivingCircle.Models;

public class AssistanceRequest
{
    public int Id { get; set; }
    [Required]
    public string UserId { get; set; } = string.Empty;
    [Required]
    [StringLength(120)]
    public string Title { get; set; } = string.Empty;
    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;
    public AssistanceCategory Category { get; set; }
    [Range(0, double.MaxValue)]
    public decimal EstimatedCost { get; set; }
    public AssistancePriority Priority { get; set; }
    public AssistanceStatus Status { get; set; } = AssistanceStatus.Draft;
    public DateTime? NeededByDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
