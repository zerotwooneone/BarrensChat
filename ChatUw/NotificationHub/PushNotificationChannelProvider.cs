using System;
using System.Threading.Tasks;
using Windows.Networking.PushNotifications;

namespace ChatUw.NotificationHub
{
    public class PushNotificationChannelProvider
    {
        private static readonly Lazy<Task<IPushNotificationChannel>> _pushNotificationChannel = new Lazy<Task<IPushNotificationChannel>>(()=>Task.Factory.StartNew(CreateChannel).Unwrap() );

        private static async Task<IPushNotificationChannel> CreateChannel()
        {
            var pushNotificationChannelForApplicationAsync = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
            return new PushNotificationChannelWrapper(pushNotificationChannelForApplicationAsync);
        }

        public virtual async Task<IPushNotificationChannel> CreateNotificationChannel()
        {
            var channel = await _pushNotificationChannel.Value;
            return channel;
        }
    }
}