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

        private async Task sendPush(string pns, string userTag, string message)
        {
            var POST_URL = BACKEND_ENDPOINT + "/api/notifications?pns=" +
                           pns + "&to_tag=" + userTag;

            using (var httpClient = new HttpClient())
            {
                var settings = ApplicationData.Current.LocalSettings.Values;
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", (string)settings["AuthenticationToken"]);

                try
                {
                    await httpClient.PostAsync(POST_URL, new StringContent("\"" + message + "\"",
                        System.Text.Encoding.UTF8, "application/json"));
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
            await SetAuthenticationTokenInLocalStorage();

            var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();

            var registerClient = new RegisterClient(BACKEND_ENDPOINT);
            await registerClient.RegisterAsync(channel.Uri, new List<string>());
        }

        private async Task SetAuthenticationTokenInLocalStorage()
        {
            var client = new Auth0Client(new Auth0ClientOptions
            {
                Domain = "zerohome.auth0.com",
                ClientId = "99BTQq42rNM4UDj05sfDLzFhsQAIJkAw"
            });

            var loginResult = await client.LoginAsync();

            ApplicationData.Current.LocalSettings.Values["AuthenticationToken"] = loginResult.IdentityToken;
        }

    }

}
