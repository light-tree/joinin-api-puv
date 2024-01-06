using Microsoft.AspNetCore.SignalR;

namespace API_JoinIn.Utils.Notification.Implements
{
    public class NotificationSignalSender : Hub
    {
        public override async Task OnConnectedAsync()
        {
            
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
           
            await base.OnDisconnectedAsync(ex);
        }
    }
}
