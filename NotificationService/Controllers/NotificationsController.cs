using CommonServices.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using NotificationService.Services;

namespace NotificationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationRepository _repo;
        private readonly IRedisNotificationCache _cache;

        public NotificationsController(INotificationRepository repo, IRedisNotificationCache cache)
        {
            _repo = repo;
            _cache = cache;
        }

        // GET /notifications/unread/{userId}
        [HttpGet("unread/{userId}")]
        public async Task<IActionResult> GetUnread(string userId)
        {
            // Try redis cache first
            var inCache = await _cache.GetUnreadAsync(userId);
            if (inCache.Any())
                return Ok(inCache);

            // Fallback to database query
            var dbList = await _repo.GetUnreadAsync(userId);
            var dtos = dbList.Select(n => new NotificationDto
            {
                Id = n.Id,
                UserId = n.UserId,
                Title = n.Title,
                Message = n.Message,
                Link = n.Link,
                CreatedAt = n.CreatedAt
            }).ToList();

            // Populate cache
            foreach (var d in dtos)
                await _cache.PushUnreadAsync(d);

            return Ok(dtos);
        }

        // POST /notifications/markread/{id}/{userId}
        [HttpPost("markread/{id}/{userId}")]
        public async Task<IActionResult> MarkRead(Guid id, string userId)
        {
            await _repo.MarkReadAsync(id);
            await _cache.RemoveByIdAsync(userId, id);
            return Ok();
        }
    }
}
