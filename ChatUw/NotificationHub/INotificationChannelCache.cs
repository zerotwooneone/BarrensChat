namespace ChatUw.NotificationHub
{
    public interface INotificationChannelCache
    {
        NotificationChannelModel GetNotificationChannel();
        void SetNotificationChannel(NotificationChannelModel notificationChannel);
    }
}