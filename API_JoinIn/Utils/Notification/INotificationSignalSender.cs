namespace API_JoinIn.Utils.Notification
{
    public interface INotificationSignalSender
    {
        public Task SendNotification(string message);
    }
}
