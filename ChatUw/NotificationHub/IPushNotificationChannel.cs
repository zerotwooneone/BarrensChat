using System;
using Windows.Foundation;
using Windows.Networking.PushNotifications;

namespace ChatUw.NotificationHub
{
    public interface IPushNotificationChannel
    {
        string Uri { get; }
        DateTimeOffset ExpirationTime { get; }
        void Close();
        event TypedEventHandler<PushNotificationChannel, PushNotificationReceivedEventArgs> PushNotificationReceived;
    }
}