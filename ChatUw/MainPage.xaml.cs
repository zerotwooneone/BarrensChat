using System;
using System.Collections.Generic;
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
        private const string BACKEND_ENDPOINT = "https://localhost:44309";

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void PushClick(object sender, RoutedEventArgs e)
        {
            if (toggleWNS.IsChecked.Value)
            {
                await sendPush("wns", ToUserTagTextBox.Text, this.NotificationMessageTextBox.Text);
            }
            if (toggleFCM.IsChecked.Value)
            {
                await sendPush("fcm", ToUserTagTextBox.Text, this.NotificationMessageTextBox.Text);
            }
            if (toggleAPNS.IsChecked.Value)
            {
                await sendPush("apns", ToUserTagTextBox.Text, this.NotificationMessageTextBox.Text);

            }
        }
        public class PublishModel   
        {
            public string pns { get; set; }
            public string  message { get; set; } 
            public string  to_tag { get; set; }
        }

        private async Task sendPush(string pns, string userTag, string message)
        {
            var httpClientFactory = new LocalHostTestHttpClientFactory();
            var settingsCache = SettingsCache.GetInstance();
            var POST_URL = $"{BACKEND_ENDPOINT}/api/Publish";

            var publishModel = new PublishModel
            {
                pns = pns,
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
                    MessageDialog alert = new MessageDialog(ex.Message, "Failed to send " + pns + " message");
                    alert.ShowAsync();
                }
            }
        }

        private async void LoginAndRegisterClick(object sender, RoutedEventArgs e)
        {
            var settingsCache = SettingsCache.GetInstance();
            await SetAuthenticationTokenInLocalStorage(settingsCache);

            var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();

            var httpClientFactory = new LocalHostTestHttpClientFactory();
            var registerClient = new RegisterClient(BACKEND_ENDPOINT, httpClientFactory, settingsCache, settingsCache);
            await registerClient.RegisterAsync(channel.Uri, new List<string>());
        }

        private async Task SetAuthenticationTokenInLocalStorage(IAuthenticationCache authenticationCache)
        {
            var client = new Auth0Client(new Auth0ClientOptions
            {
                Domain = "zerohome.auth0.com",
                ClientId = "99BTQq42rNM4UDj05sfDLzFhsQAIJkAw"
            });

            var loginResult = await client.LoginAsync();

            authenticationCache.SetAuthenticationToken(loginResult.IdentityToken);
        }

    }

}
