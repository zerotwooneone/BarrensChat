using System;
using System.Threading.Tasks;
using Windows.Networking.PushNotifications;

namespace ChatUw.NotificationHub
{
    public class PushNotificationChannelProvider
    {
        public virtual async Task<IPushNotificationChannel> CreateNotificationChannel()
        {
            var channel = new PushNotificationChannelWrapper(await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync());
            return channel;
        }
    }
}