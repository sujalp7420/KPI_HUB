using Microsoft.AspNetCore.Identity;

namespace KPI_HUB.Models.Entities;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
