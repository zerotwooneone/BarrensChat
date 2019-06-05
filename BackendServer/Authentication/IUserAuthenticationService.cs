using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BackendServer.Authentication
{
    public interface IUserAuthenticationService
    {
        Task<AddResult> Add3rdPartyUser(string username, string email);
        Task<IdentityUser> GetUserByEmail(string email);
    }
}