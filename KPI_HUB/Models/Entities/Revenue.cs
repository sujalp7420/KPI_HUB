namespace KPI_HUB.Models.Entities;

public class Revenue
{
    public int Id { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public decimal Amount { get; set; }
    public string Source { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? CustomerId { get; set; }
    public string PaymentMethod { get; set; } = "Bank Transfer";
    public string? ReferenceNumber { get; set; }

    public Customer? Customer { get; set; }
}
