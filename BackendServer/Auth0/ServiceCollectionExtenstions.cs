using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BackendServer.Auth0
{
    public static class ServiceCollectionExtenstions
    {
        public static IServiceCollection AddAuth0(this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            serviceCollection.Configure<Auth0Config>(configuration.GetSection("Auth0"));
            return serviceCollection;
        }
    }
}