namespace KPI_HUB.Models.Entities;

public class MarketingCampaign
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime EndDate { get; set; } = DateTime.UtcNow;
    public string Platform { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public decimal ActualCost { get; set; }
    public int Leads { get; set; }
    public int CustomersAcquired { get; set; }
    public decimal RevenueGenerated { get; set; }
    public string Status { get; set; } = "Active";
}
