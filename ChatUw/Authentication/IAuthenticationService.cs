using System.Threading.Tasks;

namespace ChatUw.Authentication
{
    public interface IAuthenticationService
    {
        AuthModel GetValidAuthModel();
        AuthModel SetAuthModel(string token, string username);
        Task<LoginModel> Get3rdPartyAuth();
    }

    public static class AuthenticationExtensions
    {
        public static async Task<AuthModel> GetAndSave3rdPartyAuth(this IAuthenticationService authenticationService)
        {
            var loginModel = await authenticationService.Get3rdPartyAuth();
            
            if (loginModel == null) return null;
            
            return authenticationService.SetAuthModel(loginModel.Token, loginModel.Username);

        }
    }
}