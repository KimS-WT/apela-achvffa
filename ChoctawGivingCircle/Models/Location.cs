using System.ComponentModel.DataAnnotations;

namespace ChoctawGivingCircle.Models;

public class Location
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(300)]
    public string Address { get; set; } = string.Empty;

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(200)]
    public string? Hours { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public ICollection<DonationDropOff> DropOffs { get; set; } = new List<DonationDropOff>();
}
