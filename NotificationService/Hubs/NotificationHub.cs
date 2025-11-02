using Microsoft.AspNetCore.SignalR;

namespace NotificationService.Hubs
{
    public class NotificationHub: Hub
    {
        // We'll group connections by user: client should pass ?userId=xxx during connect (for now)
        public override async Task OnConnectedAsync()
        {
            var http = Context.GetHttpContext();
            var userId = http?.Request.Query["userId"].ToString(); //Later replace userId with Context.UserIdentifier from JWT
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var http = Context.GetHttpContext();
            var userId = http?.Request.Query["userId"].ToString();
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user-{userId}");
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
