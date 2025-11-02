using CommonServices.Models;

using StackExchange.Redis;

using System.Text.Json;

//Redis helper (unread cache)

namespace NotificationService.Services
{
    public class RedisNotificationCache : IRedisNotificationCache
    {
        private readonly IDatabase _db;
        private readonly string _prefix = "notifications:unread:";
        private readonly JsonSerializerOptions _jsonOpts = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        private string Key(string userId) => $"{_prefix}{userId}";

        public RedisNotificationCache(IConnectionMultiplexer mux)
        {
            _db = mux.GetDatabase();
        }


        public async Task<List<NotificationDto>> GetUnreadAsync(string userId)
        {
            var key = Key(userId);
            var vals = await _db.ListRangeAsync(key, 0, -1);
            var list = vals.Select(v => JsonSerializer.Deserialize<NotificationDto>(v!, _jsonOpts)!).ToList();
            return list;
        }

        public async Task PushUnreadAsync(NotificationDto dto)
        {
            if (string.IsNullOrEmpty(dto.UserId)) return;
            var key = Key(dto.UserId);
            var json = JsonSerializer.Serialize(dto, _jsonOpts);
            await _db.ListLeftPushAsync(key, json);
            // optionally trim to keep only recent N
            await _db.ListTrimAsync(key, 0, 99); // keep latest 100
            // optionally set TTL
            await _db.KeyExpireAsync(key, TimeSpan.FromDays(7));
        }

        public async Task RemoveByIdAsync(string userId, Guid id)
        {
            var key = Key(userId);
            var vals = await _db.ListRangeAsync(key, 0, -1);
            foreach (var v in vals)
            {
                var dto = JsonSerializer.Deserialize<NotificationDto>(v!, _jsonOpts);
                if (dto != null && dto.Id == id)
                {
                    await _db.ListRemoveAsync(key, v);
                    break;
                }
            }
        }
    }
}
