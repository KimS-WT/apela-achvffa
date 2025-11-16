using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ChoctawGivingCircle.Data;

public class ApplicationUser : IdentityUser
{
    [StringLength(160)]
    public string? DisplayName { get; set; }

    [StringLength(500)]
    public string? Bio { get; set; }

    public bool IsTribalMember { get; set; }
    public int? TribeId { get; set; }
}
