using System.Threading.Tasks;
using ChatUw.Http;

namespace ChatUw.Backend
{
    public class HttpClientBackendClient : IBackendClient
    {
        private readonly HttpClientFactory _httpClientFactory;

        public HttpClientBackendClient(HttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<StartupResponse> AppStartup(string handle)
        {
            var c = new ColdRequestModel
            {
                DeviceInfo = new DeviceRegisterModel
                {
                    Handle = handle
                }
            };
            var client = _httpClientFactory.CreateHttpClient();
            var POST_URL = $"{MagicValues.BackendUrl}/api/AppStartup";
            var response = await client.PostJson<StartupResponseModel>(POST_URL, c);
            var result = response.Message.IsSuccessStatusCode
                ? new StartupResponse
                {
                    HubRegistration = response.Object.HubRegistration,
                    ClientAuth = response.Object.ClientAuth,
                    IsSuccess = true
                }
                : new StartupResponse
                {
                    HubRegistration = null,
                    ClientAuth = null,
                    IsSuccess = false
                };
            return result;
        }

        public async Task<RegisterUserResponse> RegisterUser(string handle, 
            string userName, 
            string registrationId,
            string thirdPartyAuthToken)
        {
            var r = new RegisterUserRequestModel
            {
                DeviceInfo = new DeviceRegisterModel
                {
                    Handle = handle
                },
                UserInfo = new UserRegisterModel
                {
                    UserName = userName
                },
                Registration = new HubRegistrationModel
                {
                    RegistrationId = registrationId
                }
            };
            var client = _httpClientFactory.CreateHttpClient(thirdPartyAuthToken);
            var POST_URL = $"{MagicValues.BackendUrl}/api/AppStartup/Register";
            var response = await client.PostJson<StartupResponseModel>(POST_URL, r);
            var result = response.Message.IsSuccessStatusCode
                ? new RegisterUserResponse
                {
                    ClientAuth = response.Object.ClientAuth,
                    HubRegistration = response.Object.HubRegistration,
                    IsSuccess = true,
                    Authentication = response.Object.Authentication
                }
                : new RegisterUserResponse
                {
                    ClientAuth = null,
                    HubRegistration = null,
                    IsSuccess = false,
                    Authentication = null
                };
            return result;
        }
    }
}