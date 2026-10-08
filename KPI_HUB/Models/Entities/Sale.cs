namespace KPI_HUB.Models.Entities;

public class Sale
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime SaleDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; } = "Cash";
    public string Status { get; set; } = "Completed";

    public Customer Customer { get; set; } = null!;
    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}
