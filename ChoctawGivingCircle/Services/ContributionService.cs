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

    public async Task CreateAsync(Contribution contribution)
    {
        await dbContext.Contributions.AddAsync(contribution);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Contribution contribution)
    {
        dbContext.Contributions.Update(contribution);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await dbContext.Contributions.FindAsync(id);
        if (entity == null)
        {
            return;
        }

        dbContext.Contributions.Remove(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task<List<Contribution>> GetByDonorAsync(string donorUserId)
    {
        return await dbContext.Contributions
            .Where(c => c.DonorUserId == donorUserId)
            .Include(c => c.AssistanceRequest)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Contribution>> GetAnonymousAsync()
    {
        return await dbContext.Contributions
            .Where(c => c.DonorUserId == null)
            .Include(c => c.AssistanceRequest)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Contribution>> GetByLocationAsync(int locationId)
    {
        return await dbContext.Contributions
            .Include(c => c.AssistanceRequest)
            .Include(c => c.DonorUser)
            .Where(c => c.Id == (dbContext.DonationDropOffs
                .Where(d => d.LocationId == locationId)
                .Select(d => d.ContributionId)
                .FirstOrDefault()))
            .ToListAsync();
    }
}
