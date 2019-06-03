using System;
using Windows.Foundation;
using Windows.Networking.PushNotifications;
using ChatUw.Annotations;

namespace ChatUw.NotificationHub
{
    public class PushNotificationChannelWrapper : IPushNotificationChannel
    {
        private readonly PushNotificationChannel _pushNotificationChannel;

        public PushNotificationChannelWrapper([NotNull] PushNotificationChannel pushNotificationChannel)
        {
            _pushNotificationChannel = pushNotificationChannel ?? throw new ArgumentNullException(nameof(pushNotificationChannel));
            _pushNotificationChannel.PushNotificationReceived += OnPushNotificationReceived;
        }

        public string Uri => _pushNotificationChannel.Uri;
        public DateTimeOffset ExpirationTime => _pushNotificationChannel.ExpirationTime;
        public void Close() => _pushNotificationChannel.Close();

        public event TypedEventHandler<PushNotificationChannel, PushNotificationReceivedEventArgs> PushNotificationReceived;

        protected virtual void OnPushNotificationReceived(PushNotificationChannel sender, PushNotificationReceivedEventArgs args)
        {
            PushNotificationReceived?.Invoke(sender, args);
        }
    }
}