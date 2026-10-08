namespace KPI_HUB.Models.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal CostPrice { get; set; }
    public int StockQuantity { get; set; }
    public int MinimumStock { get; set; }
    public string Status { get; set; } = "Active";

    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}
