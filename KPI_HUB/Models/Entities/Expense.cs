namespace KPI_HUB.Models.Entities;

public class Expense
{
    public int Id { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public decimal Amount { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string PaymentMethod { get; set; } = "Bank Transfer";
    public string? Vendor { get; set; }
    public string? Notes { get; set; }
}
