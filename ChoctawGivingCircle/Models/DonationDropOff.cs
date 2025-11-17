using System.ComponentModel.DataAnnotations;

namespace ChoctawGivingCircle.Models;

public class DonationDropOff
{
    public int Id { get; set; }

    [Required]
    public int ContributionId { get; set; }

    [Required]
    public int LocationId { get; set; }

    [Required]
    public DateTime ScheduledDate { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    [StringLength(100)]
    public string? RescheduleToken { get; set; }

    public bool IsCompleted { get; set; } = false;

    public DateTime? CompletedAt { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public Contribution? Contribution { get; set; }
    public Location? Location { get; set; }
}
