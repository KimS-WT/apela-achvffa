using System.ComponentModel.DataAnnotations;

namespace ChoctawGivingCircle.Models;

public class EmailLog
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string RecipientEmail { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string Body { get; set; } = string.Empty;

    [StringLength(50)]
    public string EmailType { get; set; } = string.Empty;

    [StringLength(50)]
    public string Status { get; set; } = "Sent"; // Sent, Failed, Pending

    [StringLength(500)]
    public string? ErrorMessage { get; set; }

    public int? RelatedContributionId { get; set; }

    public int? RelatedAssistanceRequestId { get; set; }

    [StringLength(150)]
    public string? RelatedUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation
    public Contribution? RelatedContribution { get; set; }
    public AssistanceRequest? RelatedAssistanceRequest { get; set; }
}
