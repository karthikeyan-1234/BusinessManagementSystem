using CommonServices.Models;

namespace NotificationService.Services
{
    public interface IRedisNotificationCache
    {
        Task PushUnreadAsync(NotificationDto dto);
        Task<List<NotificationDto>> GetUnreadAsync(string userId);
        Task RemoveByIdAsync(string userId, Guid id);
    }
}
