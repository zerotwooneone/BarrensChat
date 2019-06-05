using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace BackendServer.Authorization
{
    public static class ServiceCollectionExtensions
    {
        public static AuthorizationOptions AddBcAuthentication(this AuthorizationOptions authorizationOptions)
        {
            authorizationOptions.AddPolicy(BarrensChatPolicy.VerifiedEmail, VerifiedEmailPolicy);

            return authorizationOptions;
        }

        private static void VerifiedEmailPolicy(AuthorizationPolicyBuilder obj)
        {
            obj.RequireClaim(ClaimTypes.Email);
        }
    }
}