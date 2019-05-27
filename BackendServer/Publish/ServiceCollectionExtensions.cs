using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BackendServer.Publish
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPublish(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddSingleton<NotificationHubClient>(serviceProvider =>
            {
                var connectionString = configuration["PrimaryHub:FullAccessToken"];
                var hubName = configuration["PrimaryHub:Name"];
                return new NotificationHubClient(connectionString, hubName);
            });
            serviceCollection.AddSingleton<IPublishService, NotificationHubPublishService>();

            return serviceCollection;
        }
    }
}