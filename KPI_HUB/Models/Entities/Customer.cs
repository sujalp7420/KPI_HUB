namespace KPI_HUB.Models.Entities;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Active";
    public decimal TotalPurchases { get; set; }

    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    public ICollection<Revenue> Revenues { get; set; } = new List<Revenue>();
}
