using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;

namespace BackendServer.Publish
{
    public class NotificationHubPublishService : IPublishService
    {
        private readonly NotificationHubClient _notificationHubClient;

        public NotificationHubPublishService(NotificationHubClient notificationHubClient)
        {
            _notificationHubClient = notificationHubClient;
        }
        public async Task Publish()
        {
            IEnumerable<string> tags = new string[]{"test"};
            var properties = new Dictionary<string, string> {{"key", "value"}, {"time", $"{DateTime.Now}"}};
            var notificationOutcome = await _notificationHubClient.SendTemplateNotificationAsync(properties, tags);
        }
    }
}