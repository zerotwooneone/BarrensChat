using BackendServer.HubClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BackendServer.Register
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRegister(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var primary = HubClientFactory.CreatePrimary(configuration);
            serviceCollection
                .TryAddSingleton(sp=>new HubClientFactory(primary));

            return serviceCollection;
        }
    }
}