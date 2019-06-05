using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BackendServer.Authentication
{
    public class UserManagerAuthService : IUserAuthenticationService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public UserManagerAuthService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        public async Task<AddResult> Add3rdPartyUser(string username, string email)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _userManager = scope.ServiceProvider.GetService<UserManager<IdentityUser>>();
                //if (_userManager.Users.Any(u => u.UserName == username || u.Email == email))
                //    throw new Exception("User already exists with that username or email");
                var identityUser = new IdentityUser
                {
                    Email = email,
                    UserName = username
                };
                var result = await _userManager.CreateAsync(identityUser);
                return result.Succeeded
                    ? new AddResult { User = identityUser }
                    : null;    
            }
            
        }

        public async Task<IdentityUser> GetUserByEmail(string email)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _userManager = scope.ServiceProvider.GetService<UserManager<IdentityUser>>();
                return await _userManager.FindByEmailAsync(email);
            }
        }

        public async Task DeleteUserByUsername(string username)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _userManager = scope.ServiceProvider.GetService<UserManager<IdentityUser>>();
                var user = await _userManager.FindByNameAsync(username);
                var result = await _userManager.DeleteAsync(user);
            }
        }
    }
}