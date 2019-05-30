using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Net.Http;
using Windows.Storage;
using System.Net.Http.Headers;
using Windows.UI.Popups;
using System.Threading.Tasks;
using Windows.Networking.PushNotifications;
using Auth0.OidcClient;
using ChatUw.Http;
using ChatUw.Settings;
using Newtonsoft.Json;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ChatUw
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void PushClick(object sender, RoutedEventArgs e)
        {
            await sendPush(ToUserTagTextBox.Text, this.NotificationMessageTextBox.Text);
        }
        
        private async Task sendPush(string userTag, string message)
        {
            var httpClientFactory = new LocalHostTestHttpClientFactory();
            var settingsCache = SettingsCache.GetInstance();
            var POST_URL = $"{MagicValues.BackendUrl}/api/Publish";

            var publishModel = new PublishModel
            {
                message = message,
                to_tag = userTag
            };

            using (var httpClient = httpClientFactory.CreateHttpClient(settingsCache.GetAuthenticationToken()))
            {
                try
                {
                    var json = JsonConvert.SerializeObject(publishModel);
                    var content = new StringContent(json,
                        System.Text.Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(POST_URL, content);
                }
                catch (Exception ex)
                {
                    MessageDialog alert = new MessageDialog(ex.Message, "Failed to send message");
                    alert.ShowAsync();
                }
            }
        }

        private async void LoginAndRegisterClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var settingsCache = SettingsCache.GetInstance();
                await EnsureUserIsLoggedIn(settingsCache);

                var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();

                channel.PushNotificationReceived += OnPushNotificationReceived;

                var httpClientFactory = new LocalHostTestHttpClientFactory();
                var registerClient = new RegisterClient(MagicValues.BackendUrl, httpClientFactory, settingsCache, settingsCache);
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
