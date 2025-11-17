using ChoctawGivingCircle.Data;
using ChoctawGivingCircle.Models;
using Microsoft.EntityFrameworkCore;

namespace ChoctawGivingCircle.Services;

public class AssistanceRequestService(ApplicationDbContext dbContext) : IAssistanceRequestService
{
    public async Task<List<AssistanceRequest>> GetOpenRequestsAsync()
    {
        return await dbContext.AssistanceRequests
            .Where(r => r.Status == AssistanceStatus.Open || r.Status == AssistanceStatus.Approved)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<AssistanceRequest>> GetUserRequestsAsync(string userId)
    {
        return await dbContext.AssistanceRequests
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<AssistanceRequest>> GetAllAsync()
    {
        return await dbContext.AssistanceRequests
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<AssistanceRequest?> GetByIdAsync(int id)
    {
        return await dbContext.AssistanceRequests.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task CreateAsync(AssistanceRequest request)
    {
        await dbContext.AssistanceRequests.AddAsync(request);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(AssistanceRequest request)
    {
        dbContext.AssistanceRequests.Update(request);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await dbContext.AssistanceRequests.FindAsync(id);
        if (entity == null)
        {
            return;
        }

        dbContext.AssistanceRequests.Remove(entity);
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Validates if a status transition is allowed.
    /// Allowed workflow: Draft → Submitted → UnderReview → Approved/Open → FullyFunded → Fulfilled/Closed
    /// Admins can move between Open, Approved, FullyFunded, Fulfilled, and Closed at any time.
    /// </summary>
    public bool IsValidStatusTransition(AssistanceStatus currentStatus, AssistanceStatus newStatus)
    {
        // Cannot transition to same status
        if (currentStatus == newStatus)
            return false;

        // Allowed transitions from each status
        var allowedTransitions = new Dictionary<AssistanceStatus, HashSet<AssistanceStatus>>
        {
            { AssistanceStatus.Draft, new HashSet<AssistanceStatus> { AssistanceStatus.Submitted } },
            { AssistanceStatus.Submitted, new HashSet<AssistanceStatus> { AssistanceStatus.UnderReview } },
            { AssistanceStatus.UnderReview, new HashSet<AssistanceStatus> { AssistanceStatus.Approved, AssistanceStatus.Open } },
            { AssistanceStatus.Approved, new HashSet<AssistanceStatus> { AssistanceStatus.Open, AssistanceStatus.FullyFunded, AssistanceStatus.Fulfilled, AssistanceStatus.Closed } },
            { AssistanceStatus.Open, new HashSet<AssistanceStatus> { AssistanceStatus.FullyFunded, AssistanceStatus.Fulfilled, AssistanceStatus.Closed, AssistanceStatus.Approved } },
            { AssistanceStatus.FullyFunded, new HashSet<AssistanceStatus> { AssistanceStatus.Fulfilled, AssistanceStatus.Closed, AssistanceStatus.Open } },
            { AssistanceStatus.Fulfilled, new HashSet<AssistanceStatus> { AssistanceStatus.Closed } },
            { AssistanceStatus.Closed, new HashSet<AssistanceStatus> { AssistanceStatus.Open } }
        };

        if (!allowedTransitions.ContainsKey(currentStatus))
            return false;

        return allowedTransitions[currentStatus].Contains(newStatus);
    }
}
