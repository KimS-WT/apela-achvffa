using ChoctawGivingCircle.Data;
using ChoctawGivingCircle.Models;
using Microsoft.EntityFrameworkCore;

namespace ChoctawGivingCircle.Services;

public class ContributionService(ApplicationDbContext dbContext) : IContributionService
{
    public async Task<List<Contribution>> GetAllAsync()
    {
        return await dbContext.Contributions
            .Include(c => c.AssistanceRequest)
            .Include(c => c.DonorUser)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Contribution>> GetGeneralFundContributionsAsync()
    {
        return await dbContext.Contributions
            .Where(c => c.IsGeneralFund)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<Contribution?> GetByIdAsync(int id)
    {
        return await dbContext.Contributions
            .Include(c => c.AssistanceRequest)
            .Include(c => c.DonorUser)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AllocateToRequestAsync(int contributionId, int assistanceRequestId)
    {
        var contribution = await dbContext.Contributions.FindAsync(contributionId);
        if (contribution == null)
        {
            throw new InvalidOperationException("Contribution not found.");
        }

        contribution.AssistanceRequestId = assistanceRequestId;
        contribution.IsGeneralFund = false;
        await dbContext.SaveChangesAsync();
    }

    public async Task<decimal> GetGeneralFundBalanceAsync()
    {
        var fundContributions = await dbContext.Contributions
            .Where(c => c.IsGeneralFund)
            .Select(c => c.Amount)
            .ToListAsync();

        return fundContributions.Sum();
    }
}
