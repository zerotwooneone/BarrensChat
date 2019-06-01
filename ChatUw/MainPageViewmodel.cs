using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Networking.PushNotifications;
using Auth0.OidcClient;
using ChatUw.Http;
using ChatUw.Message;
using ChatUw.NotificationHub;
using ChatUw.Settings;

namespace ChatUw
{
    public class MainPageViewmodel : ViewmodelBase
    {
        private readonly IMessageViewmodelFactory _messageViewmodelFactory;
        private readonly IAuthenticationCache _authenticationCache;
        private readonly IRegistrationCache _registrationCache;
        private readonly HttpClientFactory _httpClientFactory;
        public ObservableCollection<MessageViewmodel> Messages { get; }
        public ICommand LoginCommand { get; }

        public MainPageViewmodel(IMessageViewmodelFactory messageViewmodelFactory,
            IAuthenticationCache authenticationCache, 
            IRegistrationCache registrationCache,
            HttpClientFactory httpClientFactory)
        {
            _messageViewmodelFactory = messageViewmodelFactory;
            _authenticationCache = authenticationCache;
            _registrationCache = registrationCache;
            _httpClientFactory = httpClientFactory;
            Messages = new ObservableCollection<MessageViewmodel>
            {
                new MessageViewmodel("message 1", true),
                new MessageViewmodel("message 2", false)
            };
            LoginCommand = CreateStandardUiCommend(LoginClicked);
            
        }

        private async void LoginClicked()
        {
            try
            {
                await EnsureUserIsLoggedIn(_authenticationCache);

                var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();

                channel.PushNotificationReceived += OnPushNotificationReceived;

                var registerClient = new RegisterClient(MagicValues.BackendUrl, _httpClientFactory, _authenticationCache, _registrationCache);
                await registerClient.RegisterAsync(channel.Uri, new List<string>());
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private async Task EnsureUserIsLoggedIn(IAuthenticationCache authenticationCache)
        {
            var authenticationToken = authenticationCache.GetAuthenticationToken();

            var expired = IsExpired(authenticationToken);

            if (string.IsNullOrWhiteSpace(authenticationToken) || expired)
            {
                if (await SetAuthenticationTokenInLocalStorage(authenticationCache)) return;
                throw new ApplicationException("You must log in to use this app.");
            }
        }

        private static bool IsExpired(string authenticationToken)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadToken(authenticationToken);
            var exp = jwtToken.ValidTo;
            var expired = exp < DateTime.UtcNow;
            return expired;
        }

        private void OnPushNotificationReceived(PushNotificationChannel sender, PushNotificationReceivedEventArgs args)
        {
            int x = 0;

        }

        private async Task<bool> SetAuthenticationTokenInLocalStorage(IAuthenticationCache authenticationCache)
        {
            var client = new Auth0Client(new Auth0ClientOptions
            {
                Domain = MagicValues.Auth0Domain,
                ClientId = MagicValues.Auth0ClientId
            });

            var loginResult = await client.LoginAsync();

            authenticationCache.SetAuthenticationToken(loginResult.IdentityToken);
            
            return !loginResult.IsError;
        }

        
    }
}