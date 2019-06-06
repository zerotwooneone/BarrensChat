using System.Threading.Tasks;

namespace ChatUw.Backend
{
    public interface IBackendClient
    {
        Task<StartupResponse> AppStartup(string handle);
        Task<RegisterUserResponse> RegisterUser(string handle, 
            string userName, 
            string registrationId,
            string thirdPartyAuthToken);
    }
}