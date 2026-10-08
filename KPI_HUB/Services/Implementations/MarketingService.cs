using KPI_HUB.Models.Entities;
using KPI_HUB.Models.ViewModels;
using KPI_HUB.Repositories.Interfaces;
using KPI_HUB.Services.Interfaces;

namespace KPI_HUB.Services.Implementations;

public class MarketingService : IMarketingService
{
    private readonly IMarketingCampaignRepository _campaignRepository;

    public MarketingService(IMarketingCampaignRepository campaignRepository)
    {
        _campaignRepository = campaignRepository;
    }

    public async Task<IEnumerable<MarketingCampaignListViewModel>> GetCampaignsAsync()
    {
        var campaigns = await _campaignRepository.GetAllOrderedAsync();
        return campaigns.Select(c => new MarketingCampaignListViewModel
        {
            Id = c.Id,
            Name = c.Name,
            StartDate = c.StartDate,
            EndDate = c.EndDate,
            Platform = c.Platform,
            Budget = c.Budget,
            ActualCost = c.ActualCost,
            Leads = c.Leads,
            CustomersAcquired = c.CustomersAcquired,
            RevenueGenerated = c.RevenueGenerated,
            Status = c.Status
        });
    }

    public async Task<EditMarketingCampaignViewModel?> GetCampaignForEditAsync(int id)
    {
        var campaign = await _campaignRepository.GetByIdAsync(id);
        if (campaign == null) return null;

        return new EditMarketingCampaignViewModel
        {
            Id = campaign.Id,
            Name = campaign.Name,
            StartDate = campaign.StartDate,
            EndDate = campaign.EndDate,
            Platform = campaign.Platform,
            Budget = campaign.Budget,
            ActualCost = campaign.ActualCost,
            Leads = campaign.Leads,
            CustomersAcquired = campaign.CustomersAcquired,
            RevenueGenerated = campaign.RevenueGenerated,
            Status = campaign.Status
        };
    }

    public async Task AddCampaignAsync(CreateMarketingCampaignViewModel model)
    {
        var campaign = new MarketingCampaign
        {
            Name = model.Name,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Platform = model.Platform,
            Budget = model.Budget,
            ActualCost = model.ActualCost,
            Leads = model.Leads,
            CustomersAcquired = model.CustomersAcquired,
            RevenueGenerated = model.RevenueGenerated,
            Status = model.Status
        };

        await _campaignRepository.AddAsync(campaign);
        await _campaignRepository.SaveAsync();
    }

    public async Task UpdateCampaignAsync(EditMarketingCampaignViewModel model)
    {
        var campaign = await _campaignRepository.GetByIdAsync(model.Id);
        if (campaign == null) return;

        campaign.Name = model.Name;
        campaign.StartDate = model.StartDate;
        campaign.EndDate = model.EndDate;
        campaign.Platform = model.Platform;
        campaign.Budget = model.Budget;
        campaign.ActualCost = model.ActualCost;
        campaign.Leads = model.Leads;
        campaign.CustomersAcquired = model.CustomersAcquired;
        campaign.RevenueGenerated = model.RevenueGenerated;
        campaign.Status = model.Status;

        _campaignRepository.Update(campaign);
        await _campaignRepository.SaveAsync();
    }

    public async Task DeleteCampaignAsync(int id)
    {
        var campaign = await _campaignRepository.GetByIdAsync(id);
        if (campaign == null) return;

        _campaignRepository.Delete(campaign);
        await _campaignRepository.SaveAsync();
    }
}
