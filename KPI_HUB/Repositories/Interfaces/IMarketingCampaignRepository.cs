using KPI_HUB.Models.Entities;

namespace KPI_HUB.Repositories.Interfaces;

public interface IMarketingCampaignRepository : IRepository<MarketingCampaign>
{
    Task<IEnumerable<MarketingCampaign>> GetByDateRangeAsync(DateTime start, DateTime end);
    Task<IEnumerable<MarketingCampaign>> GetAllOrderedAsync();
}
