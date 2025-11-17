using ChoctawGivingCircle.Models;

namespace ChoctawGivingCircle.Services;

public interface IContributionService
{
    Task<List<Contribution>> GetAllAsync();
    Task<List<Contribution>> GetGeneralFundContributionsAsync();
    Task<Contribution?> GetByIdAsync(int id);
    Task AllocateToRequestAsync(int contributionId, int assistanceRequestId);
    Task<decimal> GetGeneralFundBalanceAsync();
    Task CreateAsync(Contribution contribution);
    Task UpdateAsync(Contribution contribution);
    Task DeleteAsync(int id);
    Task<List<Contribution>> GetByDonorAsync(string donorUserId);
    Task<List<Contribution>> GetAnonymousAsync();
    Task<List<Contribution>> GetByLocationAsync(int locationId);
}
