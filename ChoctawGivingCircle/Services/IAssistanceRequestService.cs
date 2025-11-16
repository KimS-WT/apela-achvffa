using ChoctawGivingCircle.Models;

namespace ChoctawGivingCircle.Services;

public interface IAssistanceRequestService
{
    Task<List<AssistanceRequest>> GetOpenRequestsAsync();
    Task<List<AssistanceRequest>> GetUserRequestsAsync(string userId);
    Task<AssistanceRequest?> GetByIdAsync(int id);
    Task CreateAsync(AssistanceRequest request);
    Task UpdateAsync(AssistanceRequest request);
}
