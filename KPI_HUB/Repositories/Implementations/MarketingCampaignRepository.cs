using KPI_HUB.Data;
using KPI_HUB.Models.Entities;
using KPI_HUB.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KPI_HUB.Repositories.Implementations;

public class MarketingCampaignRepository : Repository<MarketingCampaign>, IMarketingCampaignRepository
{
    public MarketingCampaignRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<MarketingCampaign>> GetByDateRangeAsync(DateTime start, DateTime end)
    {
        return await _context.MarketingCampaigns
            .Where(m => m.StartDate >= start && m.EndDate <= end)
            .OrderByDescending(m => m.RevenueGenerated)
            .ToListAsync();
    }

    public async Task<IEnumerable<MarketingCampaign>> GetAllOrderedAsync()
    {
        return await _context.MarketingCampaigns
            .OrderByDescending(m => m.RevenueGenerated)
            .ToListAsync();
    }
}
