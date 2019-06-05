using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BackendServer.Authentication
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBcAuthentication(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .TryAddSingleton<IUserAuthenticationService, UserManagerAuthService>();

            return serviceCollection;
        }
    }
}