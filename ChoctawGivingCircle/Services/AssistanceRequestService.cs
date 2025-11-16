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
}
