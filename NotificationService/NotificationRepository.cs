using CommonServices.Models;

using Microsoft.EntityFrameworkCore;

using NotificationService.Contexts;

namespace NotificationService
{

    public interface INotificationRepository
    {
        Task AddAsync(Notification n, CancellationToken ct = default);
        Task<List<Notification>> GetUnreadAsync(string? userId);
        Task MarkReadAsync(Guid id);
    }


    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationDbContext _db;
        public NotificationRepository(NotificationDbContext db) => _db = db;

        public async Task AddAsync(Notification n, CancellationToken ct = default)
        {
            _db.Notifications.Add(n);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<List<Notification>> GetUnreadAsync(string? userId)
        {
            var q = _db.Notifications.AsQueryable().Where(x => !x.IsRead);
            if (!string.IsNullOrEmpty(userId))
                q = q.Where(x => x.UserId == userId);
            else
                q = q.Where(x => x.UserId == null);
            return await q.OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        public async Task MarkReadAsync(Guid id)
        {
            var n = await _db.Notifications.FindAsync(id);
            if (n != null)
            {
                n.IsRead = true;
                await _db.SaveChangesAsync();
            }
        }
    }
}
