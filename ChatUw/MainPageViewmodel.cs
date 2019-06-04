using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Networking.PushNotifications;
using Auth0.OidcClient;
using ChatUw.Command;
using ChatUw.Http;
using ChatUw.Message;
using ChatUw.NotificationHub;
using ChatUw.Settings;
using IdentityModel.OidcClient;

namespace ChatUw
{
    public class MainPageViewmodel : ViewmodelBase
    {
        private readonly IMessageViewmodelFactory _messageViewmodelFactory;
        private readonly IAuthenticationCache _authenticationCache;
        private readonly IRegistrationService _registrationService;
        private readonly HttpClientFactory _httpClientFactory;
        public ObservableCollection<MessageViewmodel> Messages { get; }
        public ICommand LoginCommand { get; }

        public MainPageViewmodel(IMessageViewmodelFactory messageViewmodelFactory,
            IAuthenticationCache authenticationCache, 
            IRegistrationService registrationService,
            HttpClientFactory httpClientFactory)
        {
            _messageViewmodelFactory = messageViewmodelFactory;
            _authenticationCache = authenticationCache;
            _registrationService = registrationService;
            _httpClientFactory = httpClientFactory;
            Messages = new ObservableCollection<MessageViewmodel>
            {
                new MessageViewmodel("message 1", true),
                new MessageViewmodel("message 2", false)
            };
            LoginCommand = new RelayCommand(LoginClicked, LoginEnabled);
            
        }

        private bool LoginEnabled()
        {
            var noValidReg = _registrationService.GetValidRegistrationFromCache() == null;
            var authenticationToken = _authenticationCache.GetAuthenticationToken();
            var noValidAuthToken = string.IsNullOrWhiteSpace(authenticationToken) || IsExpired(authenticationToken);
            return noValidReg &&
                noValidAuthToken; 
        }

        private async void LoginClicked()
        {
            try
            {
                var token = await EnsureUserIsLoggedIn(_authenticationCache);

                await _registrationService.CreateAndSaveRegistration(token);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private async Task<string> EnsureUserIsLoggedIn(IAuthenticationCache authenticationCache)
        {
            var authenticationToken = authenticationCache.GetAuthenticationToken();

            var expired = IsExpired(authenticationToken);

            if (!string.IsNullOrWhiteSpace(authenticationToken) && !expired) return authenticationToken;
            
            var token = await GetLoggedInToken();
            if (string.IsNullOrWhiteSpace(token))
                throw new ApplicationException("You must log in to use this app.");

            authenticationCache.SetAuthenticationToken(token);
            return token;

        }

        private static bool IsExpired(string authenticationToken)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadToken(authenticationToken);
            var exp = jwtToken.ValidTo;
            var expired = exp < DateTime.UtcNow;
            return expired;
        }
        
        private static async Task<string> GetLoggedInToken()
        {
            var loginResult = await GetLoginResult();

            var token = loginResult.IsError ? null : loginResult.IdentityToken;
            return token;
        }

        private static async Task<LoginResult> GetLoginResult()
        {
            var client = new Auth0Client(new Auth0ClientOptions
            {
                Domain = MagicValues.Auth0Domain,
                ClientId = MagicValues.Auth0ClientId,
                Scope = "openid email profile"
            });

            var loginResult = await client.LoginAsync();
            return loginResult;
        }
    }
}