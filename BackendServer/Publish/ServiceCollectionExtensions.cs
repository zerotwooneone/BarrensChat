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
                var connectionString = configuration["Publish-AccessSignature"];
                return new NotificationHubClient(connectionString,"primary");
            });
            serviceCollection.AddSingleton<IPublishService, NotificationHubPublishService>();

            return serviceCollection;
        }
    }
}