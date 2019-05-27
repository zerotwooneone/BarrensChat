using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BackendServer.HubClient;

namespace BackendServer.Publish
{
    public class NotificationHubPublishService : IPublishService
    {
        private readonly HubClientFactory _hubClientFactory;

        public NotificationHubPublishService(HubClientFactory hubClientFactory)
        {
            _hubClientFactory = hubClientFactory;
        }
        public async Task Publish()
        {
            IEnumerable<string> tags = new string[]{"test"};
            var properties = new Dictionary<string, string> {{"key", "value"}, {"time", $"{DateTime.Now}"}};
            var notificationOutcome = await _hubClientFactory.Primary.SendTemplateNotificationAsync(properties, tags);
        }
    }
}