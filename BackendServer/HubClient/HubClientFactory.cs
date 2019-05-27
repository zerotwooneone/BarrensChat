using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Configuration;

namespace BackendServer.HubClient
{
    public class HubClientFactory
    {
        public NotificationHubClient Primary { get; }

        public HubClientFactory(NotificationHubClient primary)
        {
            Primary = primary;
        }

        public static NotificationHubClient CreatePrimary(IConfiguration configuration)
        {
            var connectionString = configuration["PrimaryHub:FullAccessToken"];
            var hubName = configuration["PrimaryHub:Name"];
            var notificationHubClient = NotificationHubClient.CreateClientFromConnectionString(connectionString,
                hubName);
            return notificationHubClient;
        }
    }
}