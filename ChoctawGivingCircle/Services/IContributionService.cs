using ChoctawGivingCircle.Models;

namespace ChoctawGivingCircle.Services;

public interface IContributionService
{
    Task<List<Contribution>> GetAllAsync();
    Task<List<Contribution>> GetGeneralFundContributionsAsync();
    Task<Contribution?> GetByIdAsync(int id);
    Task AllocateToRequestAsync(int contributionId, int assistanceRequestId);
    Task<decimal> GetGeneralFundBalanceAsync();
}
