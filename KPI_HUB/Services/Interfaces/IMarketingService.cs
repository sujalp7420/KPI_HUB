using KPI_HUB.Models.ViewModels;

namespace KPI_HUB.Services.Interfaces;

public interface IMarketingService
{
    Task<IEnumerable<MarketingCampaignListViewModel>> GetCampaignsAsync();
    Task<EditMarketingCampaignViewModel?> GetCampaignForEditAsync(int id);
    Task AddCampaignAsync(CreateMarketingCampaignViewModel model);
    Task UpdateCampaignAsync(EditMarketingCampaignViewModel model);
    Task DeleteCampaignAsync(int id);
}
